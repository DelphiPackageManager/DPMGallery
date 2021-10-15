using DPMGallery.DTO;
using DPMGallery.Entities;
using DPMGallery.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;

namespace DPMGallery.Repositories
{
    public partial class SearchRepository : RepositoryBase
    {
        public async Task<ApiSearchResponse> SearchByIdsAsync(CompilerVersion compilerVersion, Platform platform, List<SearchIdDTO> ids, CancellationToken cancellationToken = default)
        {
            var sql = $@"select * from {V.SearchPackageVersion} 
                         where 
                         compiler_version = @compilerVersion
                         and platform = @platform
                         and packageid = @Id
                         and version = @Version";

            var result = new ApiSearchResponse();

            foreach (var packageIndentity in ids)
            {
                var sqlParams = new
                {
                    compilerVersion,
                    platform,
                    packageIndentity.Id,
                    packageIndentity.Version
                };

                var item = await Context.QueryFirstOrDefaultAsync<SearchResult>(sql, sqlParams, cancellationToken: cancellationToken);
                if (item != null)
                {
                    result.searchResults.Add(item);
                }
            }

            var versionIds = result.searchResults.Select(m => m.VersionId).Distinct().ToArray(); //dapper doesn't support lists for values

            var dependencies = await Context.QueryAsync<PackageDependency>($"select * from {T.PackageDependency} where packageversion_id = ANY (@versionIds)", new { versionIds }, cancellationToken: cancellationToken);

            if (dependencies.Any())
            {
                foreach (var item in result.searchResults)
                {
                    item.Dependencies = dependencies.Where(x => x.PackageVersionId == item.VersionId).ToList();
                }
            }

            result.TotalCount = result.searchResults.Count;

            return result;
        }
    }
}
