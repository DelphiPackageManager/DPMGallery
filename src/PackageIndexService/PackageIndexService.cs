using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.PackageExtraction;
using DPMGallery.Repositories;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SemanticVersion = SemanticVersioning.Version;
using System.Security.Cryptography;
using DPMGallery.Antivirus;

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
        private readonly IStorageService _storageService;
        private readonly ILogger _logger;
        private readonly ServerConfig _serverConfig;
        private readonly IAntivirusService _antivirusService;

        private const string PackageContentType = "binary/octet-stream";
        private const string DspecContentType = "application/json";
        private const string ReadmeContentType = "text/markdown";
        private const string IconContentType = "image/xyz";

        public PackageIndexService(ILogger logger, IUnitOfWork unitOfWork, ApiKeyRepository apiKeyRepository, PackageRepository packageRepository, PackageVersionRepository packageVersionRepository,
                                   PackageVersionProcessRepository packageVersionProcessRepository, TargetPlatformRepository targetPlatformRepository, PackageOwnerRepository packageOwnerRepository, 
                                   OrganisationRepository organisationRepository, IStorageService storageService, ServerConfig serverConfig, IAntivirusService antivirusService)
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
            _storageService = storageService;
            _serverConfig = serverConfig;
            _antivirusService = antivirusService;
        }

        private async Task<bool> GetIsOwner(int packageId, int userId, CancellationToken cancellationToken)
        {
            var owners = await _packageOwnerRepository.GetPackageOwners(packageId);
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


        private async Task<string> ComputePackageHash(Stream stream, CancellationToken cancellationToken)
        {
            var hashAlgo = new SHA512Managed();
            stream.Seek(0, SeekOrigin.Begin);
            var bytes = await hashAlgo.ComputeHashAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return Convert.ToBase64String(bytes);
        }

        public async Task<PackageIndexingResult> IndexAsync(Stream stream, int apiKeyId, CancellationToken cancellationToken)
        {
            Package package;
            PackageVersion packageVersion;
            PackageTargetPlatform packageTargetPlatform;

            try
            {
                using (var packageReader = new PackageArchiveReader(stream))
                {
                    (package, packageTargetPlatform, packageVersion) = packageReader.GetPackageMetaData();
                    //if we get here, the metadata is probably ok
                    bool isNew = false;

                    //if the read me is specified, try to read it from the package
                    //it should be a text or markdown file
                    if (packageVersion.HasReadMe)
                    {
                        try
                        {
                            using var readMeStream = packageReader.GetStream(packageVersion.ReadMe);
                            using StreamReader reader = new StreamReader(readMeStream);
                            packageVersion.ReadMe = reader.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"[PackageIndexService] Error loading readme from package {package.PackageId}-{packageVersion.Version}");

                        }
                    }

                    var apiKey = await _apiKeyRepository.GetApiKeyById(apiKeyId, cancellationToken);
                    if (apiKey == null)
                    {
                        return PackageIndexingResult.Error;
                    }

                    //TODO :check if there is a reserved prefix, and if so that the user is a prefix owner

                    var thePackage = await _packageRepository.GetPackageByPackageIdAsync(package.PackageId, cancellationToken);
                    if (thePackage == null)
                    {
                        isNew = true;

                        //check that the api key has permission to create package 
                        if (!apiKey.Scopes.HasFlag(ApiKeyScope.PushNewPackage))
                        {
                            return PackageIndexingResult.Forbidden;
                        }

                        //new package
                        thePackage = await _packageRepository.InsertAsync(package, cancellationToken);

                        var packageOwner = new PackageOwner()
                        {
                            OwnerId = apiKey.UserId,
                            PackageId = thePackage.Id
                        };

                        await _packageOwnerRepository.InsertAsync(packageOwner);

                    }
                    else
                    {
                        //package exists - so check if the user is a package owner. 
                        var isOwner = await GetIsOwner(thePackage.Id, apiKey.UserId, cancellationToken);
                        if (!isOwner)
                        {
                            return PackageIndexingResult.Forbidden;
                        }
                        //check that the api key actually has permissions to push a new version
                        if (!apiKey.Scopes.HasFlag(ApiKeyScope.PushPackageVersion))
                        {
                            return PackageIndexingResult.Forbidden;
                        }
                    }
                    if (thePackage == null)
                        return PackageIndexingResult.Error;

                    PackageVersion thePackageVersion;
                    PackageTargetPlatform theTargetPlatform;

                    theTargetPlatform = await _targetPlatformRepository.GetByIdCompilerPlatformAsync(thePackage.Id, packageTargetPlatform.CompilerVersion, packageTargetPlatform.Platform, cancellationToken);

                    if (theTargetPlatform == null)
                    {
                        //there has never been a packageversion for this combo of package/compiler/platform
                        packageTargetPlatform.PackageId = thePackage.Id;
                        theTargetPlatform = await _targetPlatformRepository.InsertAsync(packageTargetPlatform, cancellationToken);
                        isNew = true;
                    }

                    if (!isNew) //skip this if we created a new Package, there won't be any versions
                    {
                         thePackageVersion = await _packageVersionRepository.GetByIdAndVersion(theTargetPlatform.Id , packageVersion.Version, cancellationToken);
                         if (thePackageVersion != null)
                             return PackageIndexingResult.PackageAlreadyExists;
                    }

                    //the packaged version doesn't exist                    
                    packageVersion.TargetPlatformId = theTargetPlatform.Id;
                    packageVersion.Hash = await ComputePackageHash(stream, cancellationToken);
                    packageVersion.HashAlgorithm = "SHA512";

                    thePackageVersion = await _packageVersionRepository.InsertAsync(packageVersion, cancellationToken);

                    if (thePackageVersion == null)
                        return PackageIndexingResult.Error;

                    //update the targetplatform with the latest versions

                    SemanticVersion version = null;
                    SemanticVersion latestVer = null;
                    SemanticVersion latestStableVer = null;
                    bool updateVersions = false;

                    if (!SemanticVersion.TryParse(thePackageVersion.Version, out version))
                    {
                        _logger.Error("Package version is not a valid semantic version : {thePackageVersion.Version}");
                        return PackageIndexingResult.Error;
                    }

                    if (!version.IsPreRelease)
                    {
                        //test against the lateststable
                        if (theTargetPlatform.LatestStableVersionId.HasValue)
                        {
                            var latestStablePackageVersion = await _packageVersionRepository.GetById(theTargetPlatform.LatestStableVersionId.Value, cancellationToken);
                            if (latestStablePackageVersion != null)
                            {
                                latestStableVer = latestStablePackageVersion.SemVer;
                            }
                        }
                    }
                    else //it's prerelease
                    {
                        //test against the lateststable
                        if (theTargetPlatform.LatestVersionId.HasValue)
                        {
                            var latestPackageVersion = await _packageVersionRepository.GetById(theTargetPlatform.LatestVersionId.Value, cancellationToken);
                            if (latestPackageVersion != null)
                            {
                                latestVer = latestPackageVersion.SemVer;
                            }
                        }
                    }
                    if (!version.IsPreRelease) //stable
                    {

                        if (latestStableVer != null)
                        {
                            if (version > latestStableVer)
                            {
                                theTargetPlatform.LatestStableVersionId = thePackageVersion.Id;
                                updateVersions = true;
                            }
                        }
                        else 
                        {
                            theTargetPlatform.LatestStableVersionId = thePackageVersion.Id;
                            updateVersions = true;
                        }
                    }
                    

                    if (latestVer != null )
                    {
                        if (version > latestVer)
                        {
                            theTargetPlatform.LatestVersionId = thePackageVersion.Id;
                            updateVersions = true;
                        }
                    }
                    else 
                    {
                        theTargetPlatform.LatestVersionId = thePackageVersion.Id;
                        updateVersions = true;
                    }


                    if (updateVersions)
                    {
                        try
                        {
                            await _targetPlatformRepository.UpdateAsync(theTargetPlatform, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "[PackageIndexService] Error updating package versions");
                            return PackageIndexingResult.Error;
                        }

                    }
                    //don't rely on the filename of the upload - construct from metadata
                    string fileNameBase = $"{thePackage.PackageId}-{theTargetPlatform.FileName}-{thePackageVersion.Version}";

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
                        return PackageIndexingResult.Error;
        
                    try
                    {
                        _unitOfWork.Commit();
                        return PackageIndexingResult.Success;
                    }catch
                    {
                        //db commit failed, we want to clean up the filesystem.
                            
                        return PackageIndexingResult.Error;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[PackageIndexService] Error creating package");
                return await Task.FromResult(PackageIndexingResult.InvalidPackage);
            }

        }

        public async Task<bool> TryDeletePackageAsync(string id, SemanticVersioning.Version version, CancellationToken cancellationToken)
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
