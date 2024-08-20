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
    public class ReservedPrefixRepository : RepositoryBase
    {
        private readonly ILogger _logger;
        private readonly OrganisationRepository _organisationRepository;
        public ReservedPrefixRepository(ILogger logger, OrganisationRepository organisationRepository, IDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
            _organisationRepository = organisationRepository;
        }

        public async Task<ReservedPrefix> GetPrefixByNameAsync(string prefix, CancellationToken cancellationToken)
        {
            string sql = $"select * from {T.ReservedPrefix} where prefix = @prefix";
            return await Context.QueryFirstOrDefaultAsync<ReservedPrefix>(sql, new { prefix }, cancellationToken: cancellationToken);
        }

        public async Task<bool> GetIsReservedPrefixAsync(string prefix, CancellationToken cancellationToken)
        {
            string sql = $"select count(*) from {T.ReservedPrefix} where prefix = @prefix";
            var count = await Context.ExecuteScalarAsync<int> (sql, new { prefix }, cancellationToken: cancellationToken);
            return count > 0;
        }


        public async Task<(bool, int)> GetIsPrefixOwner(string prefix, int userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(prefix)) 
                return (false, -1);

            var reservedPrefix = await GetPrefixByNameAsync(prefix, cancellationToken);

            if (reservedPrefix != null)
            {
                //easy case
                if (reservedPrefix.OwnerId == userId)
                {
                    return (true, userId);
                }
                //might be a member of an org.
                var members = await _organisationRepository.GetMembersAsync(userId, cancellationToken);
                var member = members.FirstOrDefault(x => x.MemberId == userId);
                if (member != null)
                {
                    return (true, member.MemberId);
                }
            }
            return (false, -1);
        }
    }
}
