using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html;
using DPMGallery.Entities;
using DPMGallery.Extensions;
using DPMGallery.Extensions.Mapping;
using DPMGallery.Models;
using DPMGallery.Types;
using DPMGallery.Utils;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using NuGet.Versioning;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;



namespace DPMGallery.Repositories
{
    //Web UI Methods
    partial class SearchRepository : RepositoryBase
    {

        private async Task<List<PackageDependency>> GetPackageVersionDependencies(int[] versionIds, CancellationToken cancellationToken)
        {
            var sql = @"select packageversion_id, package_id, version_range
	                    FROM public.package_dependency
                        where packageversion_id = ANY (@versionIds)
                        order by packageversion_id, package_id";

            var results = await Context.QueryAsync<PackageDependency>(sql, new { versionIds }, cancellationToken: cancellationToken);

            return results.ToList();

        }

        private class PkgVersion
        {
            [Column("version")]
            public string Version { get; set; }

            [Column("published_utc")]
            public DateTime PublishedUtc { get; set; }

            [Column("downloads")]
            public long Downloads { get; set; }
        }

        public async Task<string> GetLatestPackageVersion(string packageId, CancellationToken cancellationToken)
        {
            var sql = $@"SELECT  latestversion
                        FROM public.search_latest_version
                        where packageid = @packageId
                        order by versionid desc
                        limit 1";
		    return await Context.ExecuteScalarAsync<string>(sql, new { packageId }, cancellationToken: cancellationToken);
		}

        public async Task<List<PackageVersionModel>> GetPackageVersions(string packageId, CancellationToken cancellationToken)
        {
            var sql = @"select pv.version, pv.published_utc, pv.downloads
                        from package p, package_version pv, package_targetplatform tp
                        where p.id = tp.package_id
                        and tp.id = pv.targetplatform_id
                        and pv.listed = true
                        and p.packageid ILIKE @packageId COLLATE ""C""
                        order by pv.version desc";

            var results = await Context.QueryAsync<PkgVersion>(sql, new { packageId }, cancellationToken: cancellationToken);

            string currentVersion = "";
            List<PackageVersionModel> versions = new List<PackageVersionModel>();
            PackageVersionModel currentModel = null;
            foreach (var item in results)
            {
                if (!string.Equals(item.Version, currentVersion, StringComparison.InvariantCultureIgnoreCase))
                {
                    currentModel = new PackageVersionModel()
                    {
                        Version = item.Version,
                        Published = item.PublishedUtc.ToPrettyDate(),
                        PublishedUtc = item.PublishedUtc
                    };
                    versions.Add(currentModel);
                    currentVersion = item.Version;
                }
                currentModel.Downloads += item.Downloads;
                
            }

            return versions;
        }

        public async Task<PackageDetailsModel> GetPackageInfo(string packageId, string version, CancellationToken cancellationToken)
        {

            var sql = @$"select * from {V.SearchPackageVersion} where 
                         packageid = @packageId
                         and version = @version
                         order by compiler_version, platform";

            var sqlParams = new
            {
                packageId,
                version
            };

            var results = await Context.QueryAsync<SearchResult>(sql, sqlParams, cancellationToken: cancellationToken);
            var entries = results.ToList();

            
            if (entries.Any())
            {
                //get all the dependencies in one sql command rather than inside the loop. 
                List<int> versionIds = entries.Select(x => x.VersionId).ToList();
                List<PackageDependency> packageDependencies = await GetPackageVersionDependencies(versionIds.ToArray(), cancellationToken);

                var firstEntry = entries.First();
                if (firstEntry != null)
                {
                    string ownersSql = @$"select 
                                      u.user_name as owner, 
                                      u.email  
                                      from {T.Users} u
                                      left join {T.PackageOwner}  o
                                      on o.owner_id = u.id
                                      where package_id = @packageId";

                    List<string> tags = string.IsNullOrEmpty(firstEntry.Tags) ? null : firstEntry.Tags.Replace(',', ' ').Split(' ').Select(x => x.Trim().ToLower()).ToList();
                    PackageDetailsModel model = new PackageDetailsModel()
                    {
                        PackageId = firstEntry.PackageId,
                        PackageVersion = firstEntry.Version,
                        Description = firstEntry.Description,
                        IsLatestVersion = firstEntry.Version == firstEntry.LatestVersion,
                        IsPrerelease = firstEntry.IsPreRelease,
                        IsCommercial = firstEntry.IsCommercial,
                        IsTrial = firstEntry.IsTrial,
                        RepositoryUrl = firstEntry.RepositoryUrl,
                        ReadMe = firstEntry.ReadMe,
                        Published = firstEntry.PublishedUtc.ToPrettyDate(),
                        PublishedUtc = firstEntry.PublishedUtc,
                        ProjectUrl = firstEntry.ProjectUrl,
                        Tags = tags,
                        PackageName = firstEntry.PackageId.ToSentenceCase(),
                        Licenses = new List<string>(firstEntry.License.Split(' ')),
                        PrefixReserved = firstEntry.IsReservedPrefix,                       
                    };

                    var owners = await Context.QueryAsync(ownersSql, new { packageId = firstEntry.Id }, cancellationToken: cancellationToken);

                    foreach (var owner in owners)
                    {
                        string email = (string)owner.email;
                        string emailHash = email.ToLower().ToMd5();
                        var newOwner = new PackageOwnerModel(email, emailHash);
						model.Owners.Add(newOwner);
                    }





                    long currentVersionDownloads = 0;

                    foreach (var item in entries)
                    {
                        model.TotalDownloads = item.TotalDownloads;
                        currentVersionDownloads += item.VersionDownloads;
                        var currentCompiler = model.CompilerPlatforms.FirstOrDefault(x => x.CompilerVersion == item.Compiler);
                        if (currentCompiler == null)
                        {
                            //new compiler platform
                            currentCompiler = new PackageDetailCompilerModel(item.Compiler);
                            model.CompilerPlatforms.Add(currentCompiler);
                        }

                        var currentPlatform = currentCompiler.Platforms.FirstOrDefault(x => x.Platform == item.Platform);
                        if (currentPlatform == null)
                        {
                            //new platform
                            currentPlatform = new PackageDetailPlatformModel(item.Platform);
                            currentCompiler.Platforms.Add(currentPlatform);
                            var dependencies = packageDependencies.Where(x => x.PackageVersionId == item.VersionId).ToList();
                            if (dependencies.Any())
                            {
                                currentPlatform.Dependencies.AddRange(dependencies.ToModel());
                            }
                        }
                    }
                    
                    model.CurrentVersionDownload = currentVersionDownloads;
                    model.Versions = await GetPackageVersions(packageId, cancellationToken);
                    return model;
                }
            }
            return null;
        }
    }
}
