using DPMGallery.Types;
using DPMGallery.Data;
using DPMGallery.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using T = DPMGallery.Constants.Database.TableNames;
using Serilog;

namespace DPMGallery.Repositories
{
    public partial class TargetPlatformRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public TargetPlatformRepository(IDbContext dbContext, ILogger logger) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<PackageTargetPlatform> GetByIdCompilerPlatformAsync(int packageId, CompilerVersion compilerVersion, Platform platform, CancellationToken cancellationToken = default )
        {
            string sql = $@"select * from {T.PackageTargetPlatform} where package_id = @packageId and compiler_version = @compilerVersion and platform = @platform";
            return await Context.QueryFirstOrDefaultAsync<PackageTargetPlatform>(sql, new { packageId, compilerVersion, platform }, cancellationToken: cancellationToken);
        }

        public async Task<PackageTargetPlatform> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            string sql = $@"select * from {T.PackageTargetPlatform} where id = @id";
            return await Context.QueryFirstOrDefaultAsync<PackageTargetPlatform>(sql, new { id }, cancellationToken: cancellationToken);
        }

        public async Task<PackageTargetPlatform> InsertAsync(PackageTargetPlatform packageTargetPlatform, CancellationToken cancellationToken = default)
        {
            string sql = $@"INSERT INTO {T.PackageTargetPlatform} (package_id, compiler_version, platform, latest_version_id, latest_version, latest_stable_version_id, latest_stable_version)
	                          VALUES (@PackageId , @CompilerVersion, @Platform, @LatestVersionId, @LatestVersion, @LatestStableVersionId, @LatestStableVersion) RETURNING id;";

            try
            {
                packageTargetPlatform.Id = await Context.ExecuteScalarAsync<int>(sql, new
                {
                    packageTargetPlatform.PackageId,
                    packageTargetPlatform.CompilerVersion,
                    packageTargetPlatform.Platform,
                    packageTargetPlatform.LatestVersionId,
                    packageTargetPlatform.LatestVersion,
                    packageTargetPlatform.LatestStableVersionId,
                    packageTargetPlatform.LatestStableVersion
                }, cancellationToken: cancellationToken);
                return packageTargetPlatform;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"[TargetPlatformRepository] AddAsync failed for {packageTargetPlatform.FileName} ");
                throw;
            }
        }

        public async Task<PackageTargetPlatform> UpdateAsync(PackageTargetPlatform packageTargetPlatform, CancellationToken cancellationToken = default)
        {

            string sql = $@"UPDATE {T.PackageTargetPlatform} 
	                        SET package_id=@PackageId, 
                                compiler_version=@CompilerVersion, 
                                platform=@Platform, 
                                latest_version_id=@LatestVersionId, 
                                latest_version = @LatestVersion,
                                latest_stable_version_id=@LatestStableVersionId,
                                latest_stable_version=@LatestStableVersion
	                        WHERE id = @Id";

            try
            {
                await Context.ExecuteAsync(sql, new
                {
                    packageTargetPlatform.Id,
                    packageTargetPlatform.PackageId,
                    packageTargetPlatform.CompilerVersion,
                    packageTargetPlatform.Platform,
                    packageTargetPlatform.LatestVersionId,
                    packageTargetPlatform.LatestVersion,
                    packageTargetPlatform.LatestStableVersionId,
                    packageTargetPlatform.LatestStableVersion
                }, cancellationToken: cancellationToken);
                return packageTargetPlatform;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[TargetPlatformRepository] UpdateAsync failed for {packageTargetPlatform}");
                         throw;
            }
        }
    }
}
