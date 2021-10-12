using System;
using DPMGallery.Data;
using Serilog;
using V = DPMGallery.Constants.Database.ViewNames;

namespace DPMGallery.Repositories
{
    public partial class SearchRepository : RepositoryBase
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
    }
}
