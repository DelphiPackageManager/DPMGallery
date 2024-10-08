﻿using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.PackageExtraction;
using DPMGallery.Repositories;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using NuGet.Versioning;
using DPMGallery.Types;
using Microsoft.VisualBasic;
using Amazon.S3.Model;

namespace DPMGallery.Services
{
    /// <summary>
    /// Handles uploading of packages.
    /// </summary>
    public class PackageIndexService : IPackageIndexService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiKeyRepository _apiKeyRepository;
        private readonly PackageRepository _packageRepository;
        private readonly TargetPlatformRepository _targetPlatformRepository;
        private readonly PackageVersionRepository _packageVersionRepository;
        private readonly PackageVersionProcessRepository _packageVersionProcessRepository;
        private readonly PackageOwnerRepository _packageOwnerRepository;
        private readonly OrganisationRepository _organisationRepository;
        private readonly ReservedPrefixRepository _reservedPrefixRepository;
        private readonly ILogger _logger;
        private readonly ServerConfig _serverConfig;


        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public PackageIndexService(ILogger logger, IUnitOfWork unitOfWork, ApiKeyRepository apiKeyRepository, PackageRepository packageRepository, PackageVersionRepository packageVersionRepository,
                                   PackageVersionProcessRepository packageVersionProcessRepository, TargetPlatformRepository targetPlatformRepository, PackageOwnerRepository packageOwnerRepository, 
                                   OrganisationRepository organisationRepository, ReservedPrefixRepository reservedPrefixRepository, ServerConfig serverConfig)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _apiKeyRepository = apiKeyRepository;
            _packageRepository = packageRepository;
            _targetPlatformRepository = targetPlatformRepository;
            _packageVersionRepository = packageVersionRepository;
            _packageVersionProcessRepository = packageVersionProcessRepository;
            _packageOwnerRepository = packageOwnerRepository;
            _organisationRepository = organisationRepository;
            _reservedPrefixRepository = reservedPrefixRepository;
            _serverConfig = serverConfig;
        }

        private async Task<bool> GetIsOwner(int packageId, int userId, CancellationToken cancellationToken)
        {
            var owners = await _packageOwnerRepository.GetPackageOwnersAsync(packageId);
            if (!owners.Any())
                return false;

            //easy one first
            var owner = owners.FirstOrDefault(x => x.OwnerId == userId);
            if (owner != null)
                return true;

            //not so easy - if any owners are organsisations and if user is member.
            int[] ids = owners.Select(x => x.OwnerId).ToArray();

            if (ids.Length == 0)
                return false;

            var orgs = await _organisationRepository.GetOrganisationsFromIdsAsync(ids, cancellationToken);

            ids = orgs.Select(x => x.Id).ToArray();

            return await _organisationRepository.GetIsMemberOfAsync(userId, ids, cancellationToken);
        }

        // See: https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/Stream.cs#L35
        private const int DefaultCopyBufferSize = 81920;

        private async Task<bool> WriteToProcessingFolderAsync(Stream content, string fileName, CancellationToken cancellationToken)
        {
            var path = Path.Combine(_serverConfig.ProcessingFolder, fileName);
            try
            {
                //overrwrite existing file. 
                using (var fileStream = File.Open(path, FileMode.OpenOrCreate))
                {
                    await content.CopyToAsync(fileStream, DefaultCopyBufferSize, cancellationToken);
                    content.Seek(0, SeekOrigin.Begin);
                }
                return true;
            } catch (Exception ex)
            {
                _logger.Error(ex, "[PackageIndexService] Error writing package file to processing folder");
                return false;
            }
        }


        private static async Task<string> ComputePackageHash(Stream stream, CancellationToken cancellationToken)
        {
            var hashAlgo = SHA256.Create();
            stream.Seek(0, SeekOrigin.Begin);
            var bytes = await hashAlgo.ComputeHashAsync(stream, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);
            return Convert.ToBase64String(bytes);
        }

        public async Task<PackageIndexingResult> IndexAsync(Stream stream, int userId,  int apiKeyId, CancellationToken cancellationToken)
        {
            Package package;
            PackageVersion packageVersion;
            PackageTargetPlatform packageTargetPlatform;

            try
            {
                using (var packageReader = new PackageArchiveReader(stream))
                {
                    (package, packageTargetPlatform, packageVersion) = packageReader.GetPackageMetaData();
                    if (packageTargetPlatform.CompilerVersion == CompilerVersion.UnknownVersion)
                    {
                        return new PackageIndexingResult()
                        {
                            Status = PackageIndexingStatus.InvalidPackage,
                            Message = "Invalid Compiler Version"
                        };

                    }

                    if (packageTargetPlatform.Platform == Platform.UnknownPlatform)
                    {
                        return new PackageIndexingResult()
                        {
                            Status = PackageIndexingStatus.InvalidPackage,
                            Message = "Invalid Platform"
                        };
                    }


                    //if we get here, the metadata is probably ok
                    bool isNew = false;

                    ////if the read me is specified, try to read it from the package
                    ////it should be a text or markdown file
                    //if (packageVersion.HasReadMe)
                    //{
                    //    try
                    //    {
                    //        using var readMeStream = packageReader.GetStream(packageVersion.ReadMe);
                    //        using StreamReader reader = new StreamReader(readMeStream);
                    //        packageVersion.ReadMe = reader.ReadToEnd();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        _logger.Error(ex, $"[PackageIndexService] Error loading readme from package {package.PackageId}-{packageVersion.Version}");

                    //    }
                    //}

                    ApiKey apiKey = null;
                    if (apiKeyId != -1)
                    {
                        apiKey = await _apiKeyRepository.GetApiKeyById(apiKeyId, cancellationToken);
                        if (apiKey == null)
                        {
                            return new PackageIndexingResult()
                            {
                                Status = PackageIndexingStatus.Forbidden,
                                Message = "I don't know who you are!"
                            };
                        }
                    }

                    Package thePackage = null;
                    
                    //we have to do this to avoid race conditions where another thread could jump in and add package or targetplatform
                    //between us checking and committing. This will slow down inserts but should not be a problem since we're unlike
                    //to get tons of concurrent inserts of the same packageid and targetplatforms (mostly in testing). 
                    await _semaphoreSlim.WaitAsync(cancellationToken);
                    try
                    {
                        thePackage = await _packageRepository.GetPackageByPackageIdAsync(package.PackageId, cancellationToken);
                        if (thePackage == null)
                        {
                            isNew = true;
                            int ownerId = userId;

                            string prefix = package.PackageId.Split(['.']).FirstOrDefault()?.ToLower();

                            if (string.IsNullOrEmpty(prefix)) 
                            {
                                return new PackageIndexingResult()
                                {
                                    Status = PackageIndexingStatus.Error,
                                    Message = "Invalid package id"
                                };

                            }

                            var reservedPrefix = await _reservedPrefixRepository.GetPrefixByNameAsync(prefix, cancellationToken);
                            if (reservedPrefix != null)
                            {
                                //check if there is a reserved prefix, and if so that the user is a prefix owner
                                //if the prefix is owned by an org this checks if user is a member of org.
                                (bool isOwner, int ownerId) prefixResult = await _reservedPrefixRepository.GetIsPrefixOwner(prefix, userId, cancellationToken);

                                if (!prefixResult.isOwner)
                                {
                                    return new PackageIndexingResult()
                                    {
                                        Status = PackageIndexingStatus.Forbidden,
                                        Message = $"You are not an owern of the Reserved Prefix : ${prefix}"
                                    };
                                }
                                ownerId = prefixResult.ownerId;
                                package.ReservedPrefix = reservedPrefix.Id;
                            }

                            //check that the api key has permission to create package 
                            //if there is no api key and the user is authenticated then they have permission
                            if (apiKey != null && !apiKey.Scopes.HasFlag(ApiKeyScope.PushNewPackage))
                            {
                                return new PackageIndexingResult()
                                {
                                    Status = PackageIndexingStatus.Forbidden,
                                    Message = "Api Key does not have the required scope (push new package)"
                                };
                                
                            }

                            //new package
                            thePackage = await _packageRepository.InsertAsync(package, cancellationToken);

                            var packageOwner = new PackageOwner()
                            {
                                OwnerId = ownerId,
                                PackageId = thePackage.Id
                            };

                            await _packageOwnerRepository.InsertAsync(packageOwner);
                            //This could throw if another request creates the package between us checking and committing. 
                            _unitOfWork.Commit();
                        }
                        else
                        {
                            //package exists - so check if the user is a package owner. 
                            //we don't need to check reserved prefix here - the packageid already exists
                            var isOwner = GetIsOwner(thePackage.Id, userId, cancellationToken).Result;
                            if (!isOwner)
                            {
                                return new PackageIndexingResult()
                                {
                                    Status = PackageIndexingStatus.Forbidden,
                                    Message = "You are not an owner of package " + package.PackageId
                                };
                                
                            }
                            //check that the api key actually has permissions to push a new version
                            if (apiKey != null && !(apiKey.Scopes.HasFlag(ApiKeyScope.PushPackageVersion) || apiKey.Scopes.HasFlag(ApiKeyScope.PushNewPackage)))
                            {
                                return new PackageIndexingResult()
                                {
                                    Status = PackageIndexingStatus.Forbidden,
                                    Message = "Api Key does not have the required scope (push new package version)"
                                };
                            }
                        }
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }

                    if (thePackage == null)
                    {
                        return new PackageIndexingResult()
                        {
                            Status = PackageIndexingStatus.Error,
                            Message = "Something went wrong creating the package db entry"
                        };
                    }

                    PackageVersion thePackageVersion;
                    PackageTargetPlatform theTargetPlatform;

                    await _semaphoreSlim.WaitAsync(cancellationToken);
                    try
                    {

                        theTargetPlatform = await _targetPlatformRepository.GetByIdCompilerPlatformAsync(thePackage.Id, packageTargetPlatform.CompilerVersion, packageTargetPlatform.Platform, cancellationToken);

                        if (theTargetPlatform == null)
                        {
                            //there has never been a packageversion for this combo of package/compiler/platform
                            packageTargetPlatform.PackageId = thePackage.Id;
                            theTargetPlatform = await _targetPlatformRepository.InsertAsync(packageTargetPlatform, cancellationToken);
                            isNew = true;
                            _unitOfWork.Commit(); //will throw early if there is an issue.
                        }
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }

                    if (!isNew) //skip this if we created a new Package, there won't be any versions
                    {
                         thePackageVersion = await _packageVersionRepository.GetByIdAndVersionAsync(theTargetPlatform.Id , packageVersion.Version, cancellationToken);
                         if (thePackageVersion != null)
                            return new PackageIndexingResult()
                            {
                                Status = PackageIndexingStatus.PackageAlreadyExists,
                                Message = "This package version already exists on the server - you cannot overwrite a pacakge, delist it and publish a new version"
                            };

                    }

                    //the packaged version doesn't exist                    
                    packageVersion.TargetPlatformId = theTargetPlatform.Id;
                    packageVersion.Hash = await ComputePackageHash(stream, cancellationToken);
                    packageVersion.HashAlgorithm = "SHA256";

                    thePackageVersion = await _packageVersionRepository.InsertAsync(packageVersion, cancellationToken);

                    if (thePackageVersion == null)
                        return new PackageIndexingResult()
                        {
                            Status = PackageIndexingStatus.Error,
                            Message = "Something went wrong writing the package version to the db"
                        };

                    
                    //don't rely on the filename of the upload - construct from metadata
                    string fileNameBase = $"{thePackage.PackageId}-{theTargetPlatform.FileName}-{thePackageVersion.Version}".ToLower();
                    _logger.Information("[PackageIndexService] processing file : {fileNameBase}", fileNameBase);
                    PackageVersionProcess pvp = new()
                    {
                        PackageVersionId = thePackageVersion.Id,
                        Completed = false,
                        LastUpdatedUtc = DateTime.UtcNow,
                        PackageFileName = fileNameBase + ".dpkg"
                    };

                    await _packageVersionProcessRepository.InsertAsync(pvp, cancellationToken);

                    //copy the file to the processing folder so we can scan it for viruses etc in a background task.
                    var copyResult = await WriteToProcessingFolderAsync(stream, fileNameBase + ".dpkg", cancellationToken);
                    if (!copyResult)
                        return new PackageIndexingResult()
                        {
                            Status = PackageIndexingStatus.Error,
                            Message = $"Something went wrong saving the package file to the processing folder"
                        };

                    try
                    {
                        _unitOfWork.Commit();
                        return new PackageIndexingResult()
                        {
                            Status = PackageIndexingStatus.Success,
                            Message = "Package queued for processing, it may take a few minutes to appear on the site"
                        };

                    }
                    catch (Exception ex)
                    {
                        //db commit failed, we want to clean up the filesystem.

                        //TODO: remove file from processing.

                        return new PackageIndexingResult()
                        {
                            Status = PackageIndexingStatus.Error,
                            Message = $"Something went wrong committing db transaction : {ex.Message}"
                        };
                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[PackageIndexService] Error creating package");
                return new PackageIndexingResult()
                {
                    Status = PackageIndexingStatus.Error,
                    Message = $"Something went wrong  : {ex.Message}"
                };
            }

        }

        public async Task<bool> TryDeletePackageAsync(string id, NuGetVersion version, CancellationToken cancellationToken)
        {
            return await Task.FromResult(false);
        }

        //public async Task<bool> ExistsAsync(string id, string version, CompilerVersion compilerVersion, Platform platform)
        //{
        //	var packageVersion = await _packageVersionRepository.GetByIdAndVersion(id, version, compilerVersion, platform);
        //	return packageVersion != null;
        //}
    }
}
