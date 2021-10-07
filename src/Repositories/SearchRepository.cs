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
                        var itemOwners = owners.Where(x => x.package_id == item.Id).Select(x => x.owner).ToArray();
                        if (itemOwners.Any())
                            item.Owners = string.Join(' ', itemOwners);
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


        public async Task<UISearchResponse> UISearchAsync(string query = null, int skip = 0, int take = 20,
                                            bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {

            var result = new UISearchResponse()
            {
                TotalCount = 0
            };

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

            result.TotalCount = await Context.ExecuteScalarAsync<int>(countSql, sqlParams , cancellationToken: cancellationToken);
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

            var ids = await Context.QueryAsync<int>(sql, idParams, cancellationToken: cancellationToken);

            if (!ids.Any())
            {
                return result;
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
            return result;
        }
    }
}
