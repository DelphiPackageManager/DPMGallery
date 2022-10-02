using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html;
using DPMGallery.Entities;
using DPMGallery.Extensions;
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

            var results = await Context.QueryAsync<PackageDependency>(sql, new { versionIds }, cancellationToken : cancellationToken);

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

            long totalDownloads = 0;
            if (entries.Any())
            {
                //get all the dependencies in one sql command rather than inside the loop. 
                List<int> versionIds = entries.Select(x => x.VersionId).ToList();
                List<PackageDependency> packageDependencies = await GetPackageVersionDependencies(versionIds.ToArray(), cancellationToken);

                var firstEntry = entries.First();
                if (firstEntry != null)
                {
                    List<string> tags = string.IsNullOrEmpty(firstEntry.Tags) ? null : firstEntry.Tags.Replace(',', ' ').Split(' ').Select(x => x.Trim().ToLower()).ToList();
                    PackageDetailsModel model = new PackageDetailsModel()
                    {
                        PackageId = firstEntry.PackageId,
                        PackageVersion = firstEntry.Version,
                        CompilerVersions = new List<CompilerVersion>(),
                        Platforms = new List<Platform>(),
                        CompilerPlatforms = new Dictionary<CompilerVersion, List<Platform>>(),
                        IsLatestVersion = firstEntry.Version == firstEntry.LatestVersion,
                        IsPrerelease = firstEntry.IsPreRelease,
                        IsCommercial = firstEntry.IsCommercial,
                        IsTrial = firstEntry.IsTrial,
                        Owners = new List<string>(),
                        RepositoryUrl = firstEntry.RepositoryUrl,
                        ReadMe = firstEntry.ReadMe,
                        Published = firstEntry.PublishedUtc.ToPrettyDate(),
                        PublishedUtc = firstEntry.PublishedUtc,
                        ProjectUrl = firstEntry.ProjectUrl,
                        //CurrentVersionDownload = firstEntry.VersionDownloads,
                        Versions = new List<PackageVersionModel>(),
                        Tags = tags,
                        PackageName = firstEntry.PackageId.ToSentenceCase(),
                        Licenses = new List<string>(firstEntry.License.Split(' ')),
                        PrefixReserved = firstEntry.IsReservedPrefix,                       
                        PackageDependencies = new Dictionary<CompilerVersion, Dictionary<Platform, List<PackageDependencyModel>>>()
                    };

                    if (firstEntry.Owners != null)
                    {
                        model.Owners.AddRange(firstEntry.Owners);
                    }

                    //Platform currentPlatform = Platform.UnknownPlatform;
                    

                    foreach (var item in entries)
                    {
                        totalDownloads += item.VersionDownloads;
                        if (!model.CompilerVersions.Contains(item.Compiler))
                        {
                            model.CompilerVersions.Add(item.Compiler);
                        }

                        if (!model.Platforms.Contains(item.Platform))
                        {
                            model.Platforms.Add(item.Platform);
                        }

                        List<Platform> compilerPlatforms = null;
                        if (!model.CompilerPlatforms.TryGetValue(item.Compiler, out compilerPlatforms))
                        {
                            compilerPlatforms = new List<Platform>();
                            model.CompilerPlatforms.Add(item.Compiler, compilerPlatforms);
                        }

                        compilerPlatforms.Add(item.Platform);

                        
                        var dependencies = packageDependencies.Where(x => x.PackageVersionId == item.VersionId).ToList();
                        if (dependencies.Any()) {

                            Dictionary<Platform, List<PackageDependencyModel>> platformDeps = null;
                            if (!model.PackageDependencies.TryGetValue(item.Compiler, out platformDeps ))
                            {
                                platformDeps = new Dictionary<Platform, List<PackageDependencyModel>>();
                                model.PackageDependencies.Add(item.Compiler, platformDeps);
                            }

                            List<PackageDependencyModel> dependencyModels = null;

                            if (!platformDeps.TryGetValue(item.Platform, out dependencyModels))
                            {
                                dependencyModels = new List<PackageDependencyModel>();        
                                platformDeps.Add(item.Platform, dependencyModels);
                            }

                            foreach (var dependency in dependencies)
                            {
                                var depModel = Mapping<PackageDependency, PackageDependencyModel>.Map(dependency);
                                dependencyModels.Add(depModel);
                            }
                        }
                    }
                    
                    model.TotalDownloads = totalDownloads;
                    model.Versions = await GetPackageVersions(packageId, cancellationToken);
                    return model;
                }
            }
            return null;
        }
    }
}
