using DPMGallery.Data;
using DPMGallery.Entities;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using DPMGallery.Statistics;

namespace DPMGallery.Repositories
{
    public class StatsRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public StatsRepository(ILogger logger, IDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<PackageDownloads>> GetTopDownloadedPackages(int max, CancellationToken cancellationToken)
        {
            string sql = $@"select packageid, downloads from package 
                            where downloads > 0
                            and active = true
                            order by downloads desc, packageid asc
                            limit @max";

            return await Context.QueryAsync<PackageDownloads>(sql, new { max }, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<VersionDownloads>> GetTopDownloadedPackageVersions(int max, CancellationToken cancellationToken)
        {
            string sql = $@"select p.packageid, pv.version, sum(pv.downloads) as downloads from 
                            package p
                            left join package_targetplatform tp on
                            p.id = tp.package_id
                            left join package_version pv on
                            pv.targetplatform_id = tp.id
                            where pv.downloads > 0
                            and p.active = true
                            group by p.packageid, pv.version
                            order by downloads desc, packageid asc
                            limit @max";

            return await Context.QueryAsync<VersionDownloads>(sql, new { max }, cancellationToken: cancellationToken);
        }

        public async Task<int> GetTotalDownloads(CancellationToken cancellationToken)
        {
            string sql = $@"select sum(downloads) from package
                            where active = true";
            return await Context.ExecuteScalarAsync<int>(sql, cancellationToken:  cancellationToken);
        }

        public async Task<int> GetUniquePackagesCount(CancellationToken cancellationToken = default)
        {
            string sql = $@"select count(distinct packageid) from package
                            where active = true";
            return await Context.ExecuteScalarAsync<int>(sql, cancellationToken: cancellationToken);
        }

        public async Task<int> GetTotalPackageVersions(CancellationToken cancellationToken)
        {
            string sql = $@"select count(*) from  (
                            select p.packageid, pv.version from 
                            package p
                            left join package_targetplatform tp on
                            p.id = tp.package_id
                            left join package_version pv on
                            pv.targetplatform_id = tp.id
                            where  p.active = true
                            group by p.packageid, pv.version
                            )
                            as packageversions";
            return await Context.ExecuteScalarAsync<int>(sql, cancellationToken: cancellationToken);
        }
    }
}
