using DPMGallery.Entities;
using DPMGallery.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using V = DPMGallery.Constants.Database.ViewNames;

namespace DPMGallery.Repositories
{
    public partial class SearchRepository : RepositoryBase
    {

        private string GetListSelectSql(bool includePrerelease)
        {

            if (includePrerelease)
            {
                return $"select packageid, compiler_version, platform, latestversion, lateststableversion from {V.SearchLatestVersion} \n";
            }
            else
            {
                return $"select packageid, compiler_version, platform, latestversion, lateststableversion from {V.SearchStableVersion} \n";
            }
        }

        private string GetListWhereSql(CompilerVersion compilerVersion, List<Platform> platforms, string query, bool exact, bool includePrerelease, bool includeCommercial, bool includeTrial)
        {
            string result = "";

            if (compilerVersion != CompilerVersion.UnknownVersion)
            {
                result = result + "and compiler_version = @compilerVersion \n";
            }
            if (platforms.Any())
            {
                result = result + "and platform = ANY (@platforms) \n";
            }

            if (!includeCommercial)
            {
                result = result + $"and is_commercial = false" + "\n";
            }
            if (!includeTrial)
            {
                result = result + $"and is_trial = false" + "\n";
            };

            if (!string.IsNullOrEmpty(query))
            {
                if (exact)
                {
                    result = result + "and packageid = @query \n";
                }
                else
                {
                    //using ILike and C collation to effect case insensitive search
                    result = result + "and packageid ILIKE @query COLLATE \"C\" \n";
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                if (result.StartsWith("and "))
                {
                    result = result.Remove(0, 4);
                }
                result = "where \n" + result;
            }

            return result;
        }


        public async Task<ApiListResponse> ListAsync(CompilerVersion compilerVersion, List<Platform> platforms, string query = null, bool exact = false, int skip = 0, int take = 20,
                                       bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            string select = GetListSelectSql(includePrerelease);
            string searchSql = GetListWhereSql(compilerVersion, platforms, query, exact, includePrerelease, includeCommercial, includeTrial);

            string countSql = GetCountSelect(includePrerelease);

            countSql = @$"{countSql}
                          {searchSql}";

            var platformsArray = platforms.Select(x => (int)x).ToArray();

            var countParams = new
            {
                compilerVersion,
                platforms = platformsArray,
                query = query != null ? exact ? query : $"%{query}%" : null
            };

            int totalCount = await Context.ExecuteScalarAsync<int>(countSql, countParams, cancellationToken: cancellationToken);

            string orderBy = "order by id, compiler_version, platform\n";
            string pagingSql = "offset @skip limit @take";

            string sql = $@"{select}
                            {searchSql}
                            {orderBy}
                            {pagingSql}";

            var sqlParams = new
            {
                compilerVersion,
                platforms = platformsArray,
                query = exact ? query : $"%{query}%",
                skip,
                take
            };

            var items = await Context.QueryAsync<ListResult>(sql, sqlParams, cancellationToken: cancellationToken);

            return new ApiListResponse()
            {
                TotalCount = totalCount,
                searchResults = items.ToList()
            };
        }
    }
}
