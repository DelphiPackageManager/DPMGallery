using DPMGallery.Data;
using DPMGallery.Entities;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using T = DPMGallery.Constants.Database.TableNames;


namespace DPMGallery.Repositories
{
    public class PackageRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public PackageRepository(ILogger logger, IDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<Package> GetPackageByPackageIdAsync(string packageid, CancellationToken cancellationToken)
        {
            string q = $"select * from {T.Package} where packageid = @packageid";
            return await Context.QueryFirstOrDefaultAsync<Package>(q, new { packageid }, cancellationToken: cancellationToken);
        }

        public async Task<Package> GetPackageByIdAsync(int id)
        {
            string q = $"select * from {T.Package} where id = @id";
            return await Context.QueryFirstOrDefaultAsync<Package>(q, new { id });
        }

        public async Task<Package> InsertAsync(Package package, CancellationToken cancellationToken)
        {

            const string insertSql = "INSERT INTO " + T.Package + " (packageid, downloads) " +
                "VALUES(@packageId, 0) RETURNING id";

            try
            {
                package.Id = await Context.ExecuteScalarAsync<int>(insertSql, new { packageId = package.PackageId }, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[PackageRepository] AddNewPackageAsync Failed for {package}");
                throw;
            }

            return package;
        }

        public async Task UpdateDownloads(string packageid, CancellationToken cancellationToken)
        {
            Package package = await GetPackageByPackageIdAsync(packageid, cancellationToken);
            if (package == null)
            {
                throw new Exception($@"Unable to update downloads for package {packageid} - package does not exist!");
            }



            //this is the more correct but db intensive way to do this, rolling up the downloads from the versions
            //we could just increment this each time a version is downloaded, but that might get out of sync if 
            //we manually edit the db.
            string totalDownloadsSql = $@"select 
                                        sum(pv.downloads)
                                        from package p
                                        inner join package_targetplatform tp on
                                        p.id = tp.package_id
                                        inner join package_version pv
                                        on pv.targetplatform_id = tp.id
                                        where pv.listed = true
                                        and p.id = @id";

            string sql = @$"UPDATE {T.Package} SET downloads = @downloads 
                           WHERE id = @id
                           RETURNING downloads";
            try
            {
                int downloads = await Context.ExecuteScalarAsync<int>(totalDownloadsSql, new { id = package.Id }, cancellationToken: cancellationToken);

                await Context.ExecuteAsync(sql, new { id = package.Id, downloads  }, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[PackageRepository] IncrementDownload Failed for id {id}");
                throw;
            }
        }
    }
        
}
