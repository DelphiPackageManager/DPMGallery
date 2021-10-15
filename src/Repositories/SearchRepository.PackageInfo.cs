using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DPMGallery.Entities;
using DPMGallery.Types;
using NuGet.Versioning;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;


namespace DPMGallery.Repositories
{
    public partial class SearchRepository : RepositoryBase
    {

        public async Task<SearchResult> GetPackageInfo(string packageId, CompilerVersion compilerVersion, Platform platform, string version, CancellationToken cancellationToken)
        {
            var sql = @$"select * from {V.SearchPackageVersion} where 
                         packageid = @packageId
                         and compiler_version = @compilerVersion
                         and platform = @platform
                         and version = @version";

            var sqlParams = new
            {
                packageId,
                compilerVersion,
                platform,
                version
            };

            var result = await Context.QueryFirstOrDefaultAsync<SearchResult>(sql, sqlParams, cancellationToken: cancellationToken);
            if (result == null)
                return null;

            var dependencies = await Context.QueryAsync<PackageDependency>($"select * from {T.PackageDependency} where packageversion_id = @packageVersionId", new { packageVersionId = result.VersionId }, cancellationToken: cancellationToken);

            if (dependencies.Any())
            {
                result.Dependencies = dependencies.ToList();
            }

            return result;
        }

        public async Task<IEnumerable<SearchResult>> GetPackageVersionsWithDependenciesAsync(string packageId, CompilerVersion compilerVersion, Platform platform, VersionRange range, bool includePrerelese, CancellationToken cancellationToken = default)
        {
            var sql = @$"select * from {V.SearchPackageVersion} where 
                         packageid = @packageId
                         and compiler_version = @compilerVersion
                         and platform = @platform
                         and listed = true
                        ";

            if (!includePrerelese)
                sql = sql + "and is_prerelease = false\n";

            sql = sql + "order by published_utc, latestversion";

            var sqlParams = new
            {
                packageId,
                compilerVersion,
                platform
            };

            var items = await Context.QueryAsync<SearchResult>(sql, sqlParams, cancellationToken: cancellationToken);

            var result = new List<SearchResult>();

            foreach (var item in items)
            {
                if (NuGetVersion.TryParseStrict(item.LatestVersion, out NuGetVersion version))
                {
                    if (range.Satisfies(version)) //TODO : Need to write our own version of this as Nuget allows all parts of a version to float, we only allow minor!
                        result.Add(item);
                }
            }


            var versionIds = result.Select(m => m.VersionId).Distinct().ToArray(); //dapper doesn't support lists for values

            var dependencies = await Context.QueryAsync<PackageDependency>($"select * from {T.PackageDependency} where packageversion_id = ANY (@versionIds)", new { versionIds }, cancellationToken: cancellationToken);

            if (dependencies.Any())
            {
                foreach (var item in result)
                {
                    item.Dependencies = dependencies.Where(x => x.PackageVersionId == item.VersionId).ToList();
                }
            }

            return result;

        }
    }
}
