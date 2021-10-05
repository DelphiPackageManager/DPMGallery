using DPMGallery.Data;
using DPMGallery.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using T = DPMGallery.Constants.Database.TableNames;

namespace DPMGallery.Repositories
{
    public class SearchRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public SearchRepository(IDbContext dbContext, ILogger logger) : base(dbContext)
        {
            _logger = logger;
        }

        private string GetTables(bool includePrerelease)
        {
            string result;
            if (includePrerelease)
            {
                result = $@"from {T.Package} p 
                            left join {T.PackageTargetPlatform} tp on
                            p.id = tp.package_id
                            left join {T.PackageVersion} pvl
                            on tp.latest_version = pvl.id
                            left join {T.PackageVersion} pv
                            on tp.latest_stable_version = pv.id
                            ";
            }
            else
            {
                result = $@"from {T.Package} p 
                            left  join {T.PackageTargetPlatform} tp on
                            p.id = tp.package_id
                            left join {T.PackageVersion} pv
                            on tp.latest_version = pv.id
                            ";
            }
            return result;
        }


        private string GetCountSelect(bool includePrerelease)
        {
            string tables = GetTables(includePrerelease);
            return  $@"select count(*)
                       {tables}" + "\n";
        }

        private string GetSelectSql(bool includePrerelease)
        {
            string tables = GetTables(includePrerelease);

            string result;

            if (includePrerelease)
            {
                result = $@"select 
                            p.id,
                            pvl.id as versionid,
                            p.packageid, 
                            tp.compiler_version, 
                            tp.platform,
                            pvl.version as latestversion,
                            pv.version as lateststableversion,
                            pvl.is_prerelease, 
                            pvl.is_commercial, 
                            pvl.is_trial, 
                            p.reserved_prefix_id is not null as is_reserved,
                            pvl.description,
                            pvl.authors,
                            pvl.icon,
                            pvl.read_me,
                            pvl.release_notes,
                            pvl.license, 
                            pvl.project_url,
                            pvl.repository_url,
                            pvl.repository_type,
                            pvl.repository_branch,
                            pvl.repository_commit,
                            pvl.listed,
                            pvl.status,
                            pvl.published_utc,
                            pvl.deprecation_state,
                            pvl.deprecation_message,
                            pvl.alternate_package,
                            pvl.hash,
                            pvl.hash_algorithm,
                            p.downloads as totaldownloads,
                            pvl.downloads as versiondownloads    
                            {tables}" + "\n";
            }
            else
            {
                result = $@"select 
                            p.id,
                            pv.id as versionid,
                            p.packageid, 
                            tp.compiler_version, 
                            tp.platform,
                            pv.version as latest_version,
                            pv.version as latest_stable_version,
                            pv.is_prerelease, 
                            pv.is_commercial, 
                            pv.is_trial, 
                            p.reserved_prefix_id is not null as is_reserved,
                            pv.description,
                            pv.authors,
                            pv.icon,
                            pv.read_me,
                            pv.release_notes,
                            pv.license, 
                            pv.project_url,
                            pv.repository_url,
                            pv.repository_type,
                            pv.repository_branch,
                            pv.repository_commit,
                            pv.listed,
                            pv.status,
                            pv.published_utc,
                            pv.deprecation_state,
                            pv.deprecation_message,
                            pv.alternate_package,
                            pv.hash,
                            pv.hash_algorithm,
                            p.downloads as total_downloads,
                            pv.downloads as version_downloads    
                            {tables}" + "\n";
            
            }

            return result;
        }

        private string GetWhereSql(CompilerVersion compilerVersion, Platform platform, string query, bool exact, bool includePrerelease, bool includeCommercial, bool includeTrial)
        {
            string result = includePrerelease ? "pvl.status = @status\n" : "pv.status = @status\n";
            
            if (compilerVersion != CompilerVersion.UnknownVersion)
            {
                result = result + "and tp.compiler_version = @compilerVersion \n";
            }
            if (platform != Platform.UnknownPlatform)
            {
                result = result + "and tp.platform = @platform \n";
            }

            string versionAlias;
            if (!includePrerelease)
            {
                versionAlias = "pv";
                result = result + "and pv.is_prerelease = false \n";
            }
            else 
            {
                versionAlias = "pvl";
            }

            if (!includeCommercial)
            {
                result = result + $"and {versionAlias}.is_commercial = false" + "\n";
            }
            if (!includeTrial)
            {
                result = result + $"and {versionAlias}.is_trial = false" + "\n";
            };

            if (!string.IsNullOrEmpty(query))
            {
                if (exact)
                {
                    result = result + "and p.packageid = @query \n";
                }
                else
                {
                    //using ILike and C collation to effect case insensitive search
                    result = result + "and p.packageid ILIKE @query COLLATE \"C\" \n";
                }
            }

            if (!string.IsNullOrEmpty(result))
                result = "where \n" + result;

            return result;
        }

        //used by the api, will not work for the UI
        public async Task<SearchResponse> SearchAsync(CompilerVersion compilerVersion, Platform platform, string query = null, bool exact = false, int skip = 0, int take = 20,
                                            bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {
            string select = GetSelectSql(includePrerelease);
            string searchSql = GetWhereSql(compilerVersion, platform,  query, exact, includePrerelease, includeCommercial, includeTrial);
            
            string countSql = GetCountSelect(includePrerelease);

            countSql = @$"{countSql}
                          {searchSql}";

            //https://stackoverflow.com/questions/6030099/does-dapper-support-the-like-operator
            var countParams = new
            {
                compilerVersion,
                platform,
                status = PackageStatus.Passed,
                query = query != null ? exact ? query : query + "%" : null
            };

            

            int totalCount = await Context.ExecuteScalarAsync<int>(countSql, countParams, cancellationToken: cancellationToken);

            string orderBy = "order by p.id, tp.compiler_version, tp.platform\n";
            string pagingSql = "offset @skip limit @take";


            string sql = $@"{select}
                            {searchSql}
                            {orderBy}
                            {pagingSql}";

            var sqlParams = new
            {
                compilerVersion,
                platform,
                query = exact? query : query + "%",
                skip,
                take,
                status = PackageStatus.Passed
            };

            var items = await Context.QueryAsync<SearchResult>(sql, sqlParams, cancellationToken: cancellationToken);



            if (items.Any())
            {
                var versionIds = items.Select(m => m.VersionId).Distinct().ToArray(); //dapper doesn't support lists for values

                var dependencies = await Context.QueryAsync<PackageDependency>($"select * from {T.PackageDependency} where packageversion_id = ANY (@versionIds)", new { versionIds }, cancellationToken : cancellationToken);

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

            return new SearchResponse()
            {
                TotalCount = totalCount,
                searchResults = items.ToList()
            };
        }

        public async Task<bool> Test(CompilerVersion compilerVersion, Platform platform, string query = null, bool exact = false, int skip = 0, int take = 20,
                                            bool includePrerelease = true, bool includeCommercial = true, bool includeTrial = true, CancellationToken cancellationToken = default)
        {

            SearchResponse searchResponse = await SearchAsync(CompilerVersion.UnknownVersion, Platform.UnknownPlatform, query, exact, skip, take,
                                            includePrerelease, includeCommercial, includeTrial, cancellationToken);

            string lastId = "";
            string latestVersion = "";
            string latestStable = "";

            //CompilerVersion lastCompiler = CompilerVersion.UnknownVersion;
            //Platform lastPlatform = Platform.UnknownPlatform;


            List<CompilerVersion> compilers = new();
            List<Platform> platforms = new();

            foreach (var result in searchResponse.searchResults)
            {
                if (result.PackageId != lastId)
                {
                    //record compilers and platform against a result item.
                 
                    //new package, start again
                    _logger.Debug("[SearchRepository] found new package {result.PackageId}");
                    lastId = result.PackageId;
                    latestVersion = result.LatestVersion;
                    latestStable = result.LatestStableVersion;
                    compilers.Clear();
                    platforms.Clear();
                    compilers.Add(result.Compiler);
                    platforms.Add(result.Platform);
                }
                else
                {
                    //if we encounter a new version, then we need to see if it is newer
                    if (latestVersion != result.LatestVersion)
                    {


                    }

                }

            }


            return false;
        }
    }

}
