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
        private string GetFindSql(string version, bool includePrerelease)
        {
            string view;
            if (!string.IsNullOrEmpty(version))
            {
                view = V.SearchPackageVersion;
            } else
            {
                view = includePrerelease ? V.SearchLatestVersion : V.SearchStableVersion;
            }

            return @$"select packageid, compiler_version, platform, version, hash, hash_algorithm 
                      from {view}";
        }

        private string GetFindWhereSql(string version)
        {
            string result = @"where packageid = @packageId
                              and compiler_version = @compilerVersion
                              and platform = @platform";

            if (!string.IsNullOrEmpty(version))
            {
                result = @$"{result}
                            and version = @version";
            }
            
            return result;
        }

        public async Task<ApiFindResponse> FindAsync(string packageId, CompilerVersion compilerVersion, Platform platform, string version, bool includePreRelease, CancellationToken cancellationToken)
        {
            string sql = GetFindSql(version, includePreRelease);
            string whereSql = GetFindWhereSql(version);

            sql = $@"{sql}
                     {whereSql}";

            var sqlParams = new
            {
                packageId , 
                compilerVersion, 
                platform, 
                version
            };

            var result = await _dbContext.QueryFirstOrDefaultAsync<ApiFindResponse>(sql, sqlParams, cancellationToken: cancellationToken);

            return result;

        }

    }
}
