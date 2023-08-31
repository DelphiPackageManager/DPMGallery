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

        /// <summary>
        /// Check if a user is a member of any of the organisations
        /// used by the package index service to determine if push is allowed.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ids"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> GetIsMemberOfAsync(int userId, int[] ids, CancellationToken cancellationToken)
        {
            if (ids.Length == 0)
                return false;

            string sql = $@"select * from {T.OrganisationMembers} where user_id = @userId and org_id in ({string.Join(",", ids)})";
            var member = await Context.QueryFirstOrDefaultAsync<OrganisationMember>(sql, new { userId }, cancellationToken: cancellationToken); ;
            return member != null;
        }

        /// <summary>
        /// check if a user is a member of an organisation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> GetIsOrgMemberAsync(int userId, int orgId, CancellationToken cancellationToken)
        {
            string sql = $"select * from {T.OrganisationMembers} where user_id = @userId and org_id = @orgId";
            var result = await Context.QueryFirstOrDefaultAsync<OrganisationMember>(sql, new { userId, orgId }, cancellationToken: cancellationToken); ;
            return result != null;
        }


        /// <summary>
        /// Check if a user is an org administrator
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> GetIsOrgAdminAsync(int userId, int orgId, CancellationToken cancellationToken)
        {
            string sql = $"select * from {T.OrganisationMembers} where user_id = @userId and org_id = @orgId and member_role = 1";
            var result = await Context.QueryFirstOrDefaultAsync<OrganisationMember>(sql, new { userId, orgId }, cancellationToken: cancellationToken); ;
            return result != null;
        }

        /// <summary>
        /// gets all the organisations a user is a member of
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserOrganisation>> GetOrganisationsForMember(int userId,  CancellationToken cancellationToken)
        {
            string sql = $@"select u.id as org_id, u.user_name as org_name, m.member_id, m.member_role from
                            {T.Users} u 
                            left join {T.OrganisationMembers} m ON m.org_id = u.id 
                            where 
                            u.is_organisation = true
                            and m.member_id = @userId";

            var result = await Context.QueryAsync<UserOrganisation>(sql, new { userId }, cancellationToken: cancellationToken);

            var ids = result.Select(x => x.OrgId).ToList();

            string packageCountSql = $@"select owner_id, count(*)
                                     from {T.PackageOwner}
                                     where owner_id = any (@ids)
                                     group by owner_id";

            var orgPackageCounts = await Context.QueryAsync(packageCountSql, new { ids }, cancellationToken: cancellationToken);

            if (orgPackageCounts.Any())
            {
                foreach (var item in result)
                {
                    item.PackageCount = orgPackageCounts.Where(x => x.ownerid == item.OrgId).Select(x => x.count).FirstOrDefault();
                }
            }

            var memberCountsSql = $@"select org_id, count(*), member_role
                                     from {T.OrganisationMembers}
                                     where org_id = any (@ids)
                                     group by org_id, member_role";

            var orgMemberCounts = await Context.QueryAsync(memberCountsSql, new { ids }, cancellationToken: cancellationToken);


            return result;
        }

        /// <summary>
        /// gets the editable details of an organiseation.
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<EditableOrganisation> GetEditableOrganisationAsync(int orgId, CancellationToken cancellationToken)
        {
            string sql = $@"select u.id, u.user_name, u.email, s.* from 
                            {T.Users} u
                            left join {T.OrganisationSettings}  s ON s.org_id = u.id
                            where u.is_organisation = true and u.id = @orgId";
            var result = await Context.QueryFirstOrDefaultAsync<EditableOrganisation>(sql, new { orgId }, cancellationToken: cancellationToken);


            return result;
        }

    
    }
}
