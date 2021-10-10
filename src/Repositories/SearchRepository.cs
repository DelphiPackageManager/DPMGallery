using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DPMGallery.Data;
using DPMGallery.Entities;
using Serilog;
using DPMGallery.Types;
using T = DPMGallery.Constants.Database.TableNames;
using V = DPMGallery.Constants.Database.ViewNames;
using SemanticVer = SemanticVersioning.Version;

namespace DPMGallery.Repositories
{
    public class SearchRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public SearchRepository(IDbContext dbContext, ILogger logger) : base(dbContext)
        {
            _logger = logger;
        }

        private string GetCountSelect(bool includePrerelease)
        {
            if (includePrerelease)
            {
                return $"select count(*) from {V.SearchLatestVersion} \n";
            }
            else
            {
                return $"select count(*) from {V.SearchStableVersion} \n";
            }
        }

        private string GetSelectSql(bool includePrerelease)
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

        private string GetWhereSql(CompilerVersion compilerVersion, Platform platform, string query, bool exact, bool includePrerelease, bool includeCommercial, bool includeTrial)
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
            string select = GetSelectSql(includePrerelease);
            string searchSql = GetWhereSql(compilerVersion, platform, query, exact, includePrerelease, includeCommercial, includeTrial);

            string countSql = GetCountSelect(includePrerelease);

            countSql = @$"{countSql}
                          {searchSql}";

            //https://stackoverflow.com/questions/6030099/does-dapper-support-the-like-operator
            var countParams = new
            {
                compilerVersion,
                platform,
                query = query != null ? exact ? query : query + "%" : null
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
                query = exact ? query : query + "%",
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
                    }
                }

            }

            return new ApiSearchResponse()
            {
                TotalCount = totalCount,
                searchResults = items.ToList()
            };
        }

        private string GetUIWhereSql(bool includeCommercial, bool includeTrial)
        {
            string result = "where id = ANY (@ids)\n";

            if (!includeCommercial)
            {
                result = result + "and is_commercial = false \n";
            }
            if (!includeTrial)
            {
                result = result + "and is_trial = false \n";
            };

            return result;
        }

        private async Task<List<int>> GetOwnerSearchIds(string owner, int skip, int take, CancellationToken cancellationToken)
        {


            string ownerIdSql = $@"select id from {T.Users} where normalized_user_name = @owner";

            var sqlParams = new
            {
                owner = owner.Trim().ToUpper()
            };

            int? ownerId = await Context.ExecuteScalarAsync<int>(ownerIdSql, sqlParams, cancellationToken: cancellationToken);
            if (!ownerId.HasValue)
            {
                return new List<int>();
            }

            string packageIdSql = $"select package_id from {T.PackageOwner} where owner_id = @ownerId";

            var idSqlParams = new
            {
                ownerId,
                skip,
                take
            };

            var ids = await Context.QueryAsync<int>(packageIdSql, idSqlParams, cancellationToken: cancellationToken);

            return ids.ToList();


        }

        private async Task<List<int>> GetTagSearchIds(string tagName, int skip, int take, bool includePrerelease, CancellationToken cancellationToken)
        {
            string view = includePrerelease ? V.SearchLatestVersion : V.SearchStableVersion;

            var idsSql = @$"select distinct id
                         from {view} 
                         where 
                         tags ILIKE @tagname COLLATE ""C""
                         offset @skip limit @take";


            var sqlParams = new
            {
                tagname = $"%{tagName}%",
                skip,
                take
            };

            var ids = await Context.QueryAsync<int>(idsSql, sqlParams, cancellationToken: cancellationToken);

            return ids.ToList();

        }

        public async Task<UISearchResponse> UISearchAsync(string query = null, int skip = 0, int take = 20,
                                            bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            var result = new UISearchResponse()
            {
                TotalCount = 0
            };

            IEnumerable<int> ids;

            if (!string.IsNullOrEmpty(query) && query.StartsWith("Tags:", StringComparison.InvariantCultureIgnoreCase))
            {
                string tagName = query.Substring(5).Trim('"').Replace(',', ' ').Trim();


                var packageIds = await GetTagSearchIds(tagName, skip, take, includePrerelease, cancellationToken);
                if (!packageIds.Any())
                    return result;

                result.TotalCount = packageIds.Count;

                ids = packageIds;
            }
            else if (!string.IsNullOrEmpty(query) && query.StartsWith("Owner:", StringComparison.InvariantCultureIgnoreCase)) 
            {
                string ownerName = query.Substring(6).Trim('"');

                var packageIds = await GetOwnerSearchIds(ownerName, skip, take, cancellationToken);
                if (!packageIds.Any())
                    return result;

                result.TotalCount = packageIds.Count;

                ids = packageIds;
            }
            else
            {
                string countSql = $"select count(*) from {T.Package}\n";

                if (!string.IsNullOrEmpty(query))
                {
                    countSql = countSql + $@"where 
                                         packageid ILIKE @query COLLATE ""C"" ";
                }


                var sqlParams = new
                {
                    query = query != null ? $"%{query}%" : null
                };

                result.TotalCount = await Context.ExecuteScalarAsync<int>(countSql, sqlParams, cancellationToken: cancellationToken);
                if (result.TotalCount == 0)
                    return result;


                string sql = $"select id from {T.Package} \n";
                if (!string.IsNullOrEmpty(query))
                {
                    sql = sql + @$"where
                               packageid ILIKE @query COLLATE ""C""";
                }
                sql = sql + @"order by id
                           offset @skip limit @take";


                var idParams = new
                {
                    query = query != null ? $"%{query}%" : null,
                    skip,
                    take
                };

                ids = await Context.QueryAsync<int>(sql, idParams, cancellationToken: cancellationToken);

                if (!ids.Any())
                {
                    return result;
                }
            }

            string versionsSql;

            if (includePrerelease)
            {

                versionsSql = $"select * from {V.SearchLatestVersion}";
            }
            else
            {
                versionsSql = $"select * from {V.SearchStableVersion}";
            }

            var whereSql = GetUIWhereSql(includeCommercial, includeTrial);

            var orderBySql = @"order by packageid, latestversion, compiler_version , platform";

            versionsSql = $@"{versionsSql} 
                             {whereSql}
                             {orderBySql}";

            var versionsParams = new
            {
                ids = ids.ToArray()
            };

            var items = await Context.QueryAsync<UISearchResult>(versionsSql, versionsParams, cancellationToken: cancellationToken);

            if (!items.Any())
                return result;

            string prevPackageId = "";
            string prevVersion = "0.0.0";

            List<CompilerVersion> compilers = new();
            List<Platform> platforms = new();
            UISearchResult currentItem = null;


            foreach (var item in items)
            {
                if (item.PackageId != prevPackageId)
                {
                    _logger.Debug("[SearchRepository] found new package {result.PackageId}");
                    if (currentItem != null)
                    {
                        currentItem.CompilerVersions = compilers.Distinct().ToList();
                        currentItem.Platforms = platforms.Distinct().ToList();
                        result.searchResults.Add(currentItem);
                    }
                    compilers.Clear();
                    platforms.Clear();

                    //new package, start again
                    currentItem = item;
                    prevPackageId = item.PackageId;
                    prevVersion = item.LatestVersion;

                    compilers.Add(item.Compiler);
                    platforms.Add(item.Platform);
                }
                else
                {
                    //if we encounter a different version, then we need to see if it is newer
                    //if it is newer, then we start again
                    if (item.LatestVersion != prevVersion)
                    {
                        SemanticVer newVer = SemanticVer.Parse(item.LatestVersion);
                        SemanticVer currentVer = SemanticVer.Parse(prevVersion);

                        if (newVer > currentVer)
                        {
                            //higher version, take this!
                            currentItem = item;
                            prevVersion = item.LatestVersion;
                            compilers.Clear();
                            platforms.Clear();
                            compilers.Add(item.Compiler);
                            platforms.Add(item.Platform);
                            continue;
                        }
                        //not newer, so we just add the compiler
                        compilers.Add(item.Compiler);
                        platforms.Add(item.Platform);
                    }
                }
            }
            if (currentItem != null)
            {
                currentItem.CompilerVersions = compilers.Distinct().ToList();
                currentItem.Platforms = platforms.Distinct().ToList();
                result.searchResults.Add(currentItem);
            }

            if (result.searchResults.Any())
            {

                string ownersSql = @$"select 
                                      u.user_name as owner, 
                                      o.package_id  
                                      from {T.Users}  u
                                      left join {T.PackageOwner}  o
                                      on o.owner_id = u.id
                                      where package_id = ANY (@packageIds)";

                var packageIds = result.searchResults.Select(m => m.Id).Distinct().ToArray();

                var owners = await Context.QueryAsync(ownersSql, new { packageIds }, cancellationToken: cancellationToken);

                if (owners.Any())
                {
                    foreach (var item in result.searchResults)
                    {
                        item.Owners = owners.Where(x => x.package_id == item.Id).Select(x => (string)x.owner).ToList();
                    }
                }
            }

            return result;
        }
    }
}
