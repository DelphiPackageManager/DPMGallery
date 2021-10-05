using DPMGallery.Data;
using DPMGallery.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Identity
{
	public class RoleStore : IQueryableRoleStore<Role>,
							 IRoleClaimStore<Role>,
							 IRoleStore<Role>
	{

		private readonly IDbContext _dbContext ;
		private readonly IUnitOfWork _unitOfWork;
		public RoleStore(IDbContext dbContext, IUnitOfWork unitOfWork)
		{
			_dbContext = dbContext;
			_unitOfWork = unitOfWork;
		}
		public IQueryable<Role> Roles
		{
			get
			{
                try
                {
                    return _dbContext.GetAll<Role>().AsQueryable();

                }
                catch //(Exception ex)
                {
                    //log error
                    throw;
                }
            }

		}

		#region RoleStore
		public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			role.Id = -1;
			const string insertSql = "INSERT INTO " + Constants.Database.TableNames.Roles + 
					                    "(name, normalized_name, concurrency_stamp) " +
										"Values(@Name, @NormalizedName, @ConcurrencyStamp) RETURNING id";
			try
			{
				role.Id = await _dbContext.ExecuteScalarAsync<int>(insertSql, new
				{
					role.Name,
					role.NormalizedName,
					role.ConcurrencyStamp

				}, cancellationToken: cancellationToken);
				_unitOfWork.Commit();
			}
			catch (Exception ex)
			{
				_unitOfWork.Rollback();
				//log error
				return IdentityResult.Failed(new IdentityError
				{
					Code = string.Empty,
					Description = $"The role with name {role.Name} could not be inserted. \n : {ex.Message}"
				});

			}

			return role.Id != -1 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
			{
				Code = string.Empty,
				Description = $"The role with name {role.Name} could not be inserted."
			});



		}

		public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
		{
			IdentityResult result;
			try
			{
				const string deleteSql = "DELETE FROM " + Constants.Database.TableNames.Roles + " WHERE id = @Id";
				//TODO : I think we need to delete from the other tables too!

				await _dbContext.ExecuteAsync(deleteSql, new { role.Id }, cancellationToken: cancellationToken);
				_unitOfWork.Commit();
				result = IdentityResult.Success;
			}
			catch (Exception ex)
			{
				_unitOfWork.Rollback();
				result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
			}

			return result;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
            try
            {
                const string findSql = "SELECY * FROM " + Constants.Database.TableNames.Roles + " WHERE id = @Id";
                int Id = int.Parse(roleId);

                return await _dbContext.QueryFirstOrDefaultAsync<Role>(findSql, new { Id }, cancellationToken: cancellationToken);

            }
            catch// (Exception ex)
            {
                //log error
                throw;
            }
        }

		public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			const string sql = "select * from " + Constants.Database.TableNames.Roles + " where normalized_name = @name";
            try
            {
                return await _dbContext.QuerySingleOrDefaultAsync<Role>(sql, new { name = normalizedRoleName }, cancellationToken: cancellationToken);

            }
            catch// (Exception ex)
            {
                //log error
                throw;
            }
        }

		public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.NormalizedName);
		}

		public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.Id.ToString());
		}

		public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.Name);
		}

		public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
		{
			role.NormalizedName = normalizedName;
			return Task.CompletedTask;
		}

		public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
		{
			role.Name = roleName;
			return Task.CompletedTask;
		}

		public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
		{
			const string deleteClaimsSQL = "delete from " + Constants.Database.TableNames.RoleClaims + " where role_id = @roleId";
			const string insertClaimsSQL = "insert into " + Constants.Database.TableNames.RoleClaims + " (role_id, claim_type, claim_value) values (RoleId, ClaimType, ClaimValue)";

            try
            {
                if (role.Claims.Any())
                {
                    await _dbContext.ExecuteAsync(deleteClaimsSQL, new { roleId = role.Id }, cancellationToken: cancellationToken);

                    await _dbContext.ExecuteAsync(insertClaimsSQL, role.Claims.Select(x => new
                    {
                        RoleId = role.Id,
                        ClaimType = x.Type,
                        ClaimValue = x.Value
                    }), cancellationToken: cancellationToken);
                }

                await _dbContext.UpdateAsync<Role>(role);
				_unitOfWork.Commit();
                return IdentityResult.Success;
            }
            catch //(Exception ex)
            {
                try
                {
					_unitOfWork.Rollback();
                }
                catch
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = nameof(UpdateAsync),
                        Description = $"Role with name {role.Name} could not be updated. Operation could not be rolled back."
                    });
                }

                return IdentityResult.Failed(new IdentityError
                {
                    Code = nameof(UpdateAsync),
                    Description = $"Role with name {role.Name} could not be updated.. Operation was rolled back."
                });
            }
        }
		#endregion Rolestore

		#region RoleClaimStore
		public async Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
		{
			role.Claims = role.Claims ?? (await GetClaimsAsync(role, cancellationToken)).ToList();
			var foundClaim = role.Claims.FirstOrDefault(x => x.Type == claim.Type);

			if (foundClaim != null)
			{
				role.Claims.Remove(foundClaim);
				role.Claims.Add(claim);
			}
			else
			{
				role.Claims.Add(claim);
			}
		}

		public async Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
		{
			role.Claims = role.Claims ?? (await GetClaimsAsync(role, cancellationToken)).ToList();
			role.Claims.Remove(claim);
		}

		public async Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default)
		{
			const string sql = "select * from " + Constants.Database.TableNames.RoleClaims + " where role_id = @roleId";

            try
            {
                var roleClaims = await _dbContext.QueryAsync<RoleClaim>(sql, new { roleId = role.Id }, cancellationToken: cancellationToken);
                return roleClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

            }
            catch //(Exception ex)
            {
                //log error
                throw;
            }
        }

		#endregion

	}
}
