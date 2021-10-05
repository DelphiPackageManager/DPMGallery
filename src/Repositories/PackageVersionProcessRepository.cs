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
    public class PackageVersionProcessRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public PackageVersionProcessRepository(IDbContext dbContext, ILogger logger) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<PackageVersionProcess>> GetNotCompleted(int take, CancellationToken cancellationToken)
        {
            string sql = $"select * from {T.PackageVersionProcess} where completed = false order by last_updated_utc desc limit @take";

            return await Context.QueryAsync<PackageVersionProcess>(sql, new { take }, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<PackageVersionProcess>> GetCompleted(int take, CancellationToken cancellationToken)
        {
            string sql = $"select * from {T.PackageVersionProcess} where completed = true order by last_updated_utc desc limit @take";

            return await Context.QueryAsync<PackageVersionProcess>(sql, new { take }, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(PackageVersionProcess packageVersionProcess, CancellationToken cancellationToken)
        {
            string sql = $"delete from {T.PackageVersionProcess} where id = @id";

            await Context.ExecuteAsync(sql, new { id = packageVersionProcess.Id }, cancellationToken: cancellationToken);
        }

        public async Task<PackageVersionProcess> InsertAsync(PackageVersionProcess pvp, CancellationToken cancellationToken)
        {
            string sql = @$"insert into {T.PackageVersionProcess} (packageversion_id, package_filename, completed, last_updated_utc, retry_count)
                            VALUES(@PackageVersionId, @PackageFileName, @Completed, @LastUpdatedUtc, @RetryCount) RETURNING id";

            pvp.Id = await Context.ExecuteScalarAsync<int>(sql, new { pvp.PackageVersionId, pvp.PackageFileName, pvp.Completed, pvp.LastUpdatedUtc, pvp.RetryCount }, cancellationToken: cancellationToken);
            return pvp;
        }

        public async Task UpdatePartialAsync(PackageVersionProcess pvp, CancellationToken cancellationToken)
        {
            string updateSql = $@"UPDATE public.package_version_process
	                              SET completed=@Completed, last_updated_utc=@LastUpdatedUtc
	                                WHERE id = @id";
            await Context.ExecuteAsync(updateSql, new { id = pvp.Id, Completed = pvp.Completed, LastUpdatedUtc = pvp.LastUpdatedUtc }, cancellationToken: cancellationToken);
        }
    }
}
