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
        private readonly IUnitOfWork _unitOfWork;
        public OrganisationRepository(ILogger logger, IDbContext dbContext) : base(dbContext)
        {
            _unitOfWork = dbContext as IUnitOfWork;
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
            string sql = $"select * from {T.OrganisationMembers} where user_id = @userId and org_id = @orgId";
            var result = await Context.QueryFirstOrDefaultAsync<OrganisationMember>(sql, new { userId, orgId }, cancellationToken: cancellationToken); ;
            return result != null && result.Role == MemberRole.Administrator;
        }

        private class PackageCount
        {
            public int OwnerId { get; set; }

            public int Count { get; set; }
        }

        private class MemberCount
        {
            public int OrgId { get; set; }
            public int Count { get; set; }
            public MemberRole Role { get; set; }
        }

        /// <summary>
        /// gets all the organisations a user is a member of
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagedList<UserOrganisation>> GetOrganisationsForMember(int userId, CancellationToken cancellationToken)
        {
            string sql = $@"select u.id as org_id, u.user_name as org_name, u.email as email, m.member_id, m.member_role, s.allow_contact, s.notify_on_publish from
                            {T.Users} u 
                            left join {T.OrganisationMembers} m ON m.org_id = u.id 
                            left join {T.OrganisationSettings} s on m.org_id = s.org_id
                            where 
                            u.is_organisation = true
                            and m.member_id = @userId";

            var qresult = await Context.QueryAsync<UserOrganisation>(sql, new { userId }, cancellationToken: cancellationToken);
            var result = qresult.ToList();

            if (result.Any())
            {
                var ids = result.Select(x => x.OrgId).Order().ToList();

                string packageCountSql = $@"select owner_id as ownerid, count(*)
                                     from {T.PackageOwner}
                                     where owner_id = any (@ids)
                                     group by owner_id";

                var orgPackageCounts = await Context.QueryAsync<PackageCount>(packageCountSql, new { ids }, cancellationToken: cancellationToken);

                if (orgPackageCounts.Any())
                {
                    foreach (var item in result)
                    {
                        item.PackageCount = orgPackageCounts.Where(x => x.OwnerId == item.OrgId).Select(x => x.Count).FirstOrDefault();
                    }
                }


                ////counts could really be done in linq.
                //var memberCountsSql = $@"select org_id as orgid, count(distinct member_id), member_role as role
                //                     from {T.OrganisationMembers}
                //                     where org_id = any (@ids)
                //                     group by org_id, member_role
                //                     order by org_id, member_role desc";

                //var orgMemberCounts = await Context.QueryAsync<MemberCount>(memberCountsSql, new { ids }, cancellationToken: cancellationToken);

                //if (orgMemberCounts.Any())
                //{
                //    foreach (var item2 in result)
                //    {
                //        var admins = orgMemberCounts.Where(x => x.OrgId == item2.OrgId && x.Role == MemberRole.Administrator).FirstOrDefault();
                //        item2.AdminCount = admins != null ? admins.Count : 0;
                //        var collabs = orgMemberCounts.Where(x => x.OrgId == item2.OrgId && x.Role == MemberRole.Collaborator).FirstOrDefault();
                //        item2.CollaboratorCount = collabs != null ? collabs.Count : 0;
                //    }
                //}

                var membersSql = $@"select m.*, u.user_name, u.email from {T.OrganisationMembers} m
                                    left join {T.Users} u ON u.id = m.member_id
                                    where org_id = any (@ids)
                                    order by org_id";
                var members = await Context.QueryAsync<OrganisationMember>(membersSql, new { ids }, cancellationToken: cancellationToken);

                if (members.Any())
                {
                    foreach (var item3 in result)
                    {
                        item3.Members = members.Where(x => x.OrgId == item3.OrgId).OrderBy(y => y.UserName).ToList();
                        item3.AdminCount = item3.Members.Where(x => x.Role == MemberRole.Administrator).Count();
                        item3.CollaboratorCount = item3.Members.Where(x => x.Role == MemberRole.Collaborator).Count();
                    }
                }
            }

            //Note for now we are cheating - the datatable react component expects PagedList so thats what we will return.

            Paging paging = new Paging()
            {
                Page = 1,
                PageSize = 99999,
                Skip = 0,

            };

            return new PagedList<UserOrganisation>(result, result.Count, paging);

        }

        public async Task<bool> CheckUserExists(string userName, CancellationToken cancellationToken)
        {
            string normalizedUser = userName.ToUpper();
            string sql = $@"select count(*) from {T.Users}
                            where normalized_user_name = @normalizedUser";

            var count = await Context.ExecuteScalarAsync<int>(sql, new { normalizedUser }, cancellationToken: cancellationToken);

            return count > 0;

        }


        ///// <summary>
        ///// gets the editable details of an organiseation.
        ///// </summary>
        ///// <param name="orgId"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //public async Task<EditableOrganisation> GetEditableOrganisationAsync(int orgId, CancellationToken cancellationToken)
        //{
        //    string sql = $@"select u.id, u.user_name, u.email, s.* from 
        //                    {T.Users} u
        //                    left join {T.OrganisationSettings}  s ON s.org_id = u.id
        //                    where u.is_organisation = true and u.id = @orgId";
        //    var result = await Context.QueryFirstOrDefaultAsync<EditableOrganisation>(sql, new { orgId }, cancellationToken: cancellationToken);


        //    return result;
        //}

        public async Task<bool> AddMemberToOrganisation(OrganisationMember member, CancellationToken cancellationToken)
        {
            string  insertSql = $@"insert into {T.OrganisationMembers} 
                VALUES(@OrgId, @MemberId, @Role)";

            try
            {
                var sqlParams = new
                {
                    member.OrgId,
                    member.MemberId,
                    member.Role
                };

                var rowsAffected = await Context.ExecuteAsync(insertSql, sqlParams, cancellationToken: cancellationToken);
                _unitOfWork.Commit();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.Error(ex, "[OrganisationRepository] AddMemberToOrganisation failed for Org : {orgId}  User : {userId}", [member.OrgId, member.MemberId]);
                throw;
            }
        }        

        public async Task<IEnumerable<OrganisationMember>> GetMembersAsync(int orgId, CancellationToken cancellationToken)
        {
            var membersSql = $@"select m.*, u.user_name, u.email from {T.OrganisationMembers} m
                                    left join {T.Users} u ON u.id = m.member_id
                                    where org_id = @orgId
                                    order by u.user_name";

            try
            {
                var members = await Context.QueryAsync<OrganisationMember>(membersSql, new { orgId }, cancellationToken: cancellationToken);
                return members;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[OrganisationRepository] GetMembersAsync failed for Org : {orgId}", [orgId]);
                throw;
            }
        }

        public async Task<UserOrganisation> GetOrganisationAsync(int orgId, CancellationToken cancellationToken)
        {
            string sql = $@"select u.id as org_id, u.user_name as org_name, u.email as email, m.member_id, m.member_role, s.allow_contact, s.notify_on_publish from
                            {T.Users} u 
                            left join {T.OrganisationSettings} s on u.org_id = s.org_id
                            where 
                            u.id = @orgId";

            var org =  await Context.QueryFirstOrDefaultAsync<UserOrganisation>(sql, new { orgId }, cancellationToken: cancellationToken);

            var members = await GetMembersAsync(orgId, cancellationToken: cancellationToken);

            org.Members = members.ToList();
            return org;
        }
    }
}
