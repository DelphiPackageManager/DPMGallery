using DPMGallery.Entities;
using DPMGallery.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;
using NuGet.Versioning;

namespace DPMGallery.Repositories
{
    public partial class SearchRepository : RepositoryBase
    {
        private static string GetSearchSelectSql(bool includePrerelease)
        {
            if (includePrerelease)
            {
                return $"select * from {V.SearchLatestVersion} \n";
            }
            else
            {
                return $"select * from {V.SearchStableVersion} \n";
            }
        }


        private static string GetSearchWhereSql(CompilerVersion compilerVersion, Platform platform, string query, bool exact, bool includePrerelease, bool includeCommercial, bool includeTrial)
        {
            string result = "";

            if (compilerVersion != CompilerVersion.UnknownVersion)
            {
                result = result + "and compiler_version = @compilerVersion \n";
            }
            if (platform != Platform.UnknownPlatform)
            {
                result = result + "and platform = @platform \n";
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

        //used by the api, will not work for the UI
        public async Task<ApiSearchResponse> SearchAsync(CompilerVersion compilerVersion, Platform platform, string query = null, bool exact = false, int skip = 0, int take = 20,
                                            bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            string select = GetSearchSelectSql(includePrerelease);
            string searchSql = GetSearchWhereSql(compilerVersion, platform, query, exact, includePrerelease, includeCommercial, includeTrial);

            string countSql = GetCountSelect(includePrerelease);

            countSql = @$"{countSql}
                          {searchSql}";

            //https://stackoverflow.com/questions/6030099/does-dapper-support-the-like-operator
            var countParams = new
            {
                compilerVersion,
                platform,
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
                platform,
                query = exact ? query : $"%{query}%",
                skip,
                take
            };

            var items = await Context.QueryAsync<SearchResult>(sql, sqlParams, cancellationToken: cancellationToken);

            if (items.Any())
            {
                var versionIds = items.Select(m => m.VersionId).Distinct().ToArray(); //dapper doesn't support lists for values

                var dependencies = await Context.QueryAsync<PackageDependency>($"select * from {T.PackageDependency} where packageversion_id = ANY (@versionIds)", new { versionIds }, cancellationToken: cancellationToken);

                if (dependencies.Any())
                {
                    foreach (var item in items)
                    {
                        item.Dependencies = dependencies.Where(x => x.PackageVersionId == item.VersionId).ToList();
                    }
                }

                string ownersSql = @$"select 
                                      u.user_name as owner,
                                      u.email as email,
                                      o.package_id  
                                      from {T.Users}  u
                                      left join {T.PackageOwner}  o
                                      on o.owner_id = u.id
                                      where package_id = ANY (@packageIds)";

                var packageIds = items.Select(m => m.Id).Distinct().ToArray();

                var owners = await Context.QueryAsync(ownersSql, new { packageIds }, cancellationToken: cancellationToken);

                if (owners.Any())
                {
                    foreach (var item in items)
                    {
                        item.Owners = owners.Where(x => x.package_id == item.Id).Select(x => (string)x.owner).ToList();
                        item.OwnerInfos = owners.Where(x => x.package_id == item.Id).Select(x => new PackageOwnerInfo((string)x.owner, (string)x.email) ).ToList();
					}
                }

            }

            return new ApiSearchResponse()
            {
                TotalCount = totalCount,
                searchResults = items.ToList()
            };
        }


    }
}
