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
    public class OrganisationRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        public OrganisationRepository(ILogger logger, IDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Organisation>> GetOrganisationsFromIdsAsync(int[] ids, CancellationToken cancellationToken)
        {
            string sql = $@"select id, user_name from {T.Users} where is_organisation = true and id in ({string.Join(",", ids)})";

            return await Context.QueryAsync<Organisation>(sql, cancellationToken: cancellationToken);
        }

        public async Task<bool> GetIsMemberOfAsync(int userId, int[] ids, CancellationToken cancellationToken)
        {
            if (ids.Length == 0)
                return false;

            string sql = $@"select * from {T.OrganisationMember} where user_id = @userId and org_id in ({string.Join(",", ids)})";
            var member = await Context.QueryFirstOrDefaultAsync<OrganisationMember>(sql, new { userId }, cancellationToken: cancellationToken); ;
            return member != null;
        }

        public async Task<OrganisationMember> GetIsMemberAsync(int userId, int orgId, CancellationToken cancellationToken)
        {
            string sql = $"select * from {T.OrganisationMember} where user_id = @userId and org_id = @orgId";
            return await Context.QueryFirstOrDefaultAsync<OrganisationMember>(sql, new { userId, orgId }, cancellationToken: cancellationToken); ;
        }
    }
}
