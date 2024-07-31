using DPMGallery.Data;
using DPMGallery.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using T = DPMGallery.Constants.Database.TableNames;




namespace DPMGallery.Repositories
{
    public class PackageOwnerRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public PackageOwnerRepository(ILogger logger, IDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

        //used by packageindex service
        public async Task<IEnumerable<PackageOwner>> GetPackageOwners(int packageId)
        {
            string sql = $"select * from {T.PackageOwner} where package_id = @packageId";
            return await Context.QueryAsync<PackageOwner>(sql, new { packageId });
        }

        public async Task<PackageOwner> InsertAsync(PackageOwner packageOwner)
        {
            string sql = @$"INSERT INTO {T.PackageOwner} (package_id, owner_id) VALUES(@PackageId, @OwnerId);";

            try
            {
                await Context.ExecuteAsync(sql, new { packageOwner.PackageId, packageOwner.OwnerId });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[PackageOwnerRepository] AddOwner Failed for {packageOwner}");
                throw;
            }
            return packageOwner;
        }
    }
}
