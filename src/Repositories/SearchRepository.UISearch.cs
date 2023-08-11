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
using System.Web;

namespace DPMGallery.Repositories
{
    //used by the DPM Gallery UI
    public partial class SearchRepository : RepositoryBase
    {
        private string GetUIPackageTypeSql(bool includePrerelease, bool includeCommercial, bool includeTrial)
        {
            string result = "";
            if (!includePrerelease)
            {
                result = result + "and is_prerelease = false \n";
            }
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

        private string GetUIViewName(bool includePrerelease)
        {
            return includePrerelease ? V.SearchLatestVersion : V.SearchStableVersion;
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

        private async Task<Tuple<long, List<int>>> GetTagSearchIds(string tagName, bool includePrerelease, int skip, int take, CancellationToken cancellationToken)
        {
            string view = includePrerelease ? V.SearchLatestVersion : V.SearchStableVersion;

            var countSql = @$"select count(distinct id)
                         from {view} 
                         where 
                         tags ILIKE @tagname COLLATE ""C""";

            var countParams = new
            {
                tagname = $"%{tagName}%"
            };


            long count = await Context.ExecuteScalarAsync<long>(countSql, countParams, cancellationToken: cancellationToken);


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

            return new Tuple<long, List<int>>(count, ids.ToList());

        }

        private async Task<Tuple<long, List<int>>> GetOwnerSearchIds(string owner, int skip, int take, CancellationToken cancellationToken)
        {
            var sqlParams = new
            {
                owner = owner.Trim().ToUpper()
            };

            string ownerIdSql = $@"select id from {T.Users} where normalized_user_name = @owner";

            int? ownerId = await Context.ExecuteScalarAsync<int>(ownerIdSql, sqlParams, cancellationToken: cancellationToken);
            if (!ownerId.HasValue)
            {
                return new Tuple<long, List<int>>(0, new List<int>());
            }

            string packageIdCountSql = $@"select count(*) from {T.PackageOwner} where owner_id = @ownerId";


            long totalPackages = await Context.ExecuteScalarAsync<long>(packageIdCountSql, new { ownerId } , cancellationToken: cancellationToken);

            string packageIdSql = @$"select package_id from {T.PackageOwner} where owner_id = @ownerId
                                     offset @skip limit @take";

            var idSqlParams = new
            {
                ownerId,
                skip,
                take
            };

            var ids = await Context.QueryAsync<int>(packageIdSql, idSqlParams, cancellationToken: cancellationToken);

            return new Tuple<long, List<int>>(totalPackages, ids.ToList());
        }


        //public async Task<UISearchResponse> UISearchAsync(string query = null, int skip = 0, int take = 20, bool includePrerelease = true, bool includeCommercial = true,
        //                                          bool includeTrial = true, CancellationToken cancellationToken = default)
        //{
        //    var result = new UISearchResponse()
        //    {
        //        TotalCount = 0
        //    };

        //    IEnumerable<UISearchResult> items = new List<UISearchResult>();
        //    string packageTypeWhereSql = GetUIPackageTypeSql(includePrerelease, includeCommercial, includeTrial);
        //    string viewName = GetUIViewName(includePrerelease);
        //    //filter by tags
        //    if (!string.IsNullOrEmpty(query) && query.StartsWith("tag:", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        string tagName = query.Substring(4).Trim('"').Replace(',', ' ').Trim();

        //        var countSql = @$"select count(distinct id)
        //                 from {viewName} 
        //                 where 
        //                 tags ILIKE @tagname COLLATE ""C""
        //                 {packageTypeWhereSql}";

        //        var countParams = new
        //        {
        //            tagname = $"%{tagName}%"
        //        };

        //        result.TotalCount = await Context.ExecuteScalarAsync<long>(countSql, countParams, cancellationToken: cancellationToken);
        //        if (result.TotalCount == 0)
        //            return result;

        //        var searchSql = @$"select *  from {viewName} 
        //                 where 
        //                 tags ILIKE @tagname COLLATE ""C""
        //                 {packageTypeWhereSql}";

        //        var sqlParams = new
        //        {
        //            tagname = $"%{tagName}%",
        //            skip,
        //            take
        //        };

        //        items = await Context.QueryAsync<UISearchResult>(searchSql, countParams, cancellationToken: cancellationToken);


        //    } //filter by package owner
        //    else if (!string.IsNullOrEmpty(query) && query.StartsWith("owner:", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        string ownerName = query.Substring(6).Trim('"').ToUpper();
        //        string ownerIdSql = $@"select id from {T.Users} where normalized_user_name = @ownerName";

        //        var sqlParams = new
        //        {
        //            ownerName
        //        };

        //        int? ownerId = await Context.ExecuteScalarAsync<int?>(ownerIdSql, sqlParams, cancellationToken: cancellationToken);
        //        if (ownerId.HasValue)
        //        {
        //            //get the total count
        //           
        //            string ownerCountSql = @$"select count(distinct id) from {viewName}
        //                                    where owner_id = @owner_id
        //                                    {packageTypeWhereSql}";
        //            var countParams = new
        //            {
        //                owner_id = ownerId.Value
        //            };

        //            result.TotalCount = await Context.ExecuteScalarAsync<long>(ownerCountSql, countParams, cancellationToken: cancellationToken);
        //            if (result.TotalCount == 0)
        //                return result;

        //            string searchSql = $@"select * from {viewName}
        //                                    where owner_id = @owner_id
        //                                    {packageTypeWhereSql}";

        //            items = await Context.QueryAsync<UISearchResult>(searchSql, countParams, cancellationToken: cancellationToken);

        //        }

        //    } else //filter by regular query
        //    {
        //        string whereSql = "";

        //        string countSql = $@"select count(distinct id) from {viewName}";
        //        if (!string.IsNullOrEmpty(query))
        //        {
        //            whereSql = $@"where packageid ILIKE @query COLLATE ""C""
        //                         {packageTypeWhereSql}";
        //        } else
        //        {
        //            if (!string.IsNullOrEmpty(packageTypeWhereSql))
        //            {
        //                //
        //                if (packageTypeWhereSql.StartsWith("and"))
        //                {
        //                    //remove the and as we don't have another part of where
        //                    packageTypeWhereSql = packageTypeWhereSql.Substring(3);
        //                }

        //                whereSql =  $@"where {packageTypeWhereSql}";
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(whereSql))
        //        {
        //            countSql = countSql + $" {whereSql}";
        //        }

        //        var sqlParams = new
        //        {
        //            query = query != null ? $"%{query}%" : null
        //        };

        //        result.TotalCount = await Context.ExecuteScalarAsync<int>(countSql, sqlParams, cancellationToken: cancellationToken);
        //        if (result.TotalCount == 0)
        //            return result;


        //    }

        //    if (items == null || !items.Any())
        //    {
        //        result.TotalCount = 0;
        //        return result;
        //    }

        //    string prevPackageId = "";
        //    string prevVersion = "0.0.0";

        //    List<CompilerVersion> compilers = new();
        //    List<Platform> platforms = new();
        //    UISearchResult currentItem = null;


        //    foreach (var item in items)
        //    {
        //        if (item.PackageId != prevPackageId)
        //        {
        //            _logger.Debug("[SearchRepository] found new package {result.PackageId}");
        //            if (currentItem != null)
        //            {
        //                currentItem.CompilerVersions = compilers.Distinct().ToList();
        //                currentItem.Platforms = platforms.Distinct().ToList();
        //                result.searchResults.Add(currentItem);
        //            }
        //            compilers.Clear();
        //            platforms.Clear();

        //            //new package, start again
        //            currentItem = item;
        //            prevPackageId = item.PackageId;
        //            prevVersion = item.LatestVersion;

        //            compilers.Add(item.Compiler);
        //            platforms.Add(item.Platform);
        //        }
        //        else
        //        {
        //            //if we encounter a different version, then we need to see if it is newer
        //            //if it is newer, then we start again
        //            if (item.LatestVersion != prevVersion)
        //            {
        //                NuGetVersion newVer = NuGetVersion.Parse(item.LatestVersion);
        //                NuGetVersion currentVer = NuGetVersion.Parse(prevVersion);

        //                if (newVer > currentVer)
        //                {
        //                    //higher version, take this!
        //                    currentItem = item;
        //                    prevVersion = item.LatestVersion;
        //                    compilers.Clear();
        //                    platforms.Clear();
        //                    compilers.Add(item.Compiler);
        //                    platforms.Add(item.Platform);
        //                    continue;
        //                }
        //                //not newer, so we just add the compiler
        //                compilers.Add(item.Compiler);
        //                platforms.Add(item.Platform);
        //            }
        //            else
        //            {
        //                compilers.Add(item.Compiler);
        //                platforms.Add(item.Platform);
        //            }
        //        }
        //    }
        //    if (currentItem != null)
        //    {
        //        currentItem.CompilerVersions = compilers.Distinct().ToList();
        //        currentItem.Platforms = platforms.Distinct().ToList();
        //        result.searchResults.Add(currentItem);
        //    }

        //    if (result.searchResults.Any())
        //    {

        //        string ownersSql = @$"select 
        //                              u.user_name as owner, 
        //                              o.package_id  
        //                              from {T.Users}  u
        //                              left join {T.PackageOwner}  o
        //                              on o.owner_id = u.id
        //                              where package_id = ANY (@packageIds)";

        //        var packageIds = result.searchResults.Select(m => m.Id).Distinct().ToArray();

        //        var owners = await Context.QueryAsync(ownersSql, new { packageIds }, cancellationToken: cancellationToken);

        //        if (owners.Any())
        //        {
        //            foreach (var item in result.searchResults)
        //            {
        //                item.Owners = owners.Where(x => x.package_id == item.Id).Select(x => (string)x.owner).ToList();
        //            }
        //        }
        //    }

        //    return result;


        //    return result;
        //}


        public async Task<UISearchResponse> UISearchAsync(string query = null, int skip = 0, int take = 20, bool includePrerelease = true, bool includeCommercial = true, 
                                                          bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            var result = new UISearchResponse()
            {
                TotalCount = 0
            };

   

             IEnumerable<int> ids;
            //filter by tags
            if (!string.IsNullOrEmpty(query) && query.StartsWith("tag:", StringComparison.InvariantCultureIgnoreCase))
            {
                string tagName = query.Substring(4).Trim('"').Replace(',', ' ').Trim();


                var packageIds = await GetTagSearchIds(tagName, includePrerelease, skip, take,  cancellationToken);
                if (!packageIds.Item2.Any())
                    return result;

                result.TotalCount = packageIds.Item1;

                ids = packageIds.Item2;
            }
            //filter by package owner.
            else if (!string.IsNullOrEmpty(query) && query.StartsWith("owner:", StringComparison.InvariantCultureIgnoreCase))
            {
                string ownerName = query.Substring(6).Trim('"');

                var ownerSearchIds = await GetOwnerSearchIds(ownerName, skip, take, cancellationToken);
                if (!ownerSearchIds.Item2.Any())
                    return result;

                result.TotalCount = ownerSearchIds.Item1;
                
                ids = ownerSearchIds.Item2;
            }
            else
            {

                string countSql = @$"SELECT count ( distinct p.id)
                             FROM {T.Package} p
                             LEFT JOIN {T.PackageTargetPlatform} tp ON p.id = tp.package_id
                             LEFT JOIN {T.PackageVersion}  pvl ON tp.id = pvl.targetplatform_id
                             WHERE pvl.status = 200 AND pvl.listed = true";


                if (!string.IsNullOrEmpty(query))
                {
                    countSql = countSql + $@"
                                         and p.packageid ILIKE @query COLLATE ""C"" ";
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
                        NuGetVersion newVer = NuGetVersion.Parse(item.LatestVersion);
                        NuGetVersion currentVer = NuGetVersion.Parse(prevVersion);

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
                    } else
                    {
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