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
    public class ReservedPrefixRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public ReservedPrefixRepository(ILogger logger, IDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<ReservedPrefix>> GetPrefixOwnersAsync(string prefix)
        {
            string sql = $"select * from {T.ReservedPrefix} where prefix = @prefix";
            return (IEnumerable<ReservedPrefix>)await Context.QueryAsync<ReservedPrefix>(sql, new { prefix });
        }

    }
}
