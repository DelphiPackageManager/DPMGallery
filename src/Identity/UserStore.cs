using DPMGallery.Data;
using DPMGallery.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System.Xml.Linq;

namespace DPMGallery.Identity
{

	//This is based on https://github.com/simonfaltum/AspNetCore.Identity.Dapper


	//Note : only only CreateAsync, UpdateAsync and DeleteAsync actually need to write to the database.


	/*
	IQueryableUserStore<ApplicationUser>, 
	IUserEmailStore<ApplicationUser>, 
	IUserLoginStore<ApplicationUser>, 
	IUserPasswordStore<ApplicationUser>,
	IUserPhoneNumberStore<ApplicationUser>, 
	IUserTwoFactorStore<ApplicationUser>, 
	IUserSecurityStampStore<ApplicationUser>, 
	IUserClaimStore<ApplicationUser>,
	IUserLockoutStore<ApplicationUser>, 
	IUserRoleStore<ApplicationUser>, 
	IUserAuthenticationTokenStore<ApplicationUser>, 
	IUserStore<ApplicationUser>	  
	 
	*/

	public class UserStore : IQueryableUserStore<User>,
							 IUserEmailStore<User>,
							 IUserLoginStore<User>,
							 IUserPasswordStore<User>,
							 IUserTwoFactorStore<User>,
							 IUserSecurityStampStore<User>,
							 IUserClaimStore<User>,
							 IUserLockoutStore<User>,
							 IUserRoleStore<User>,
							 IUserAuthenticationTokenStore<User>,
							 IUserPhoneNumberStore<User>,
							 IUserAuthenticatorKeyStore<User>,
							 IUserTwoFactorRecoveryCodeStore<User>,
							 IUserStore<User>,
							 IProtectedUserStore<User>
							 

	{

		private readonly IDbContext _dbContext;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRoleStore<Role> _roleStore;
		private readonly ILogger _logger;

		public UserStore(IDbContext dbContext, ILogger logger, IUnitOfWork unitOfWork, IRoleStore<Role> roleStore)
		{
			_dbContext = dbContext;
			_roleStore = roleStore;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		#region IQueryableUserStore<User>
		public IQueryable<User> Users
		{
			get
			{
                try
                {
                    return _dbContext.GetAll<User>().AsQueryable();

                }
                catch (Exception ex)
                {
					_logger.Error(ex, "[UserStore] Error querying Users");
                    throw;
                }
            }
		}

		#endregion


		#region IUserStore<User>
		public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
		{
			IdentityResult result;
			try
			{
				//await _dbContext.InsertAsync<User>(user); //doesn't work as dapper.contrib doesn't use column mapping!

				const string createSql = "INSERT INTO " + Constants.Database.TableNames.Users + " " +
					"(user_name, normalized_user_name, email, normalized_email, email_confirmed, password_hash, security_stamp, concurrency_stamp, phone_number, phone_number_confirmed, two_factor_enabled," +
					" lockout_end, lockout_enabled, access_failed_count, is_organisation, account_suspended, refresh_token, refresh_token_expiry) " +  
					"VALUES (@UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, " +
							"@PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount, @IsOrganisation, @AccountSuspended, " +
                            "@RefreshToken, @RefreshTokenExpiryTime) RETURNING id;";

				user.Id = await _dbContext.ExecuteScalarAsync<int>(createSql, new
				{
					user.UserName,
					user.NormalizedUserName,
					user.Email,
					user.NormalizedEmail,
					user.EmailConfirmed,
					user.PasswordHash,
					user.SecurityStamp,
					user.ConcurrencyStamp,
					user.PhoneNumber,
					user.PhoneNumberConfirmed,
					user.TwoFactorEnabled,
					user.LockoutEnd,
					user.LockoutEnabled,
					user.AccessFailedCount,
					user.IsOrganisation,
					user.AccountSuspended,
					user.RefreshToken,
					user.RefreshTokenExpiryTime
				}, cancellationToken: cancellationToken);
				_unitOfWork.Commit();
				result = IdentityResult.Success;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[CreateAsync] Error Creating User");
				IdentityError[] errors = { new IdentityError() { Description = ex.Message } };
				result = IdentityResult.Failed(errors);
				_unitOfWork.Rollback();
			}

			return result;

		}


		public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
		{
			IdentityResult result;
			try
			{

				//await _dbContext.DeleteAsync<User>(user); //doesn't work with postgresql

				const string deleteUserRolesSql = "delete from " + Constants.Database.TableNames.UserRoles + " where user_id = @userId";
				await _dbContext.ExecuteAsync(deleteUserRolesSql, new { userId = user.Id }, cancellationToken: cancellationToken);

				const string deleteUserClaimsSql = "delete from " + Constants.Database.TableNames.UserClaims + " where user_id = @userId";
				await _dbContext.ExecuteAsync(deleteUserClaimsSql, new { userId = user.Id }, cancellationToken: cancellationToken);

				const string deleteUserLoginsSql = "delete from " + Constants.Database.TableNames.UserLogins + " where user_id = @userId";
				await _dbContext.ExecuteAsync(deleteUserLoginsSql, new { userId = user.Id }, cancellationToken: cancellationToken);

				const string deleteUserTokensSql = "delete from " + Constants.Database.TableNames.UserTokens + " where user_id = @userId";
				await _dbContext.ExecuteAsync(deleteUserTokensSql, new { userId = user.Id }, cancellationToken: cancellationToken);

				const string deleteUserSql = "delete from " + Constants.Database.TableNames.Users + " where id = @Id";
				await _dbContext.ExecuteAsync(deleteUserSql, new { user.Id }, cancellationToken: cancellationToken);


				_unitOfWork.Commit();
				result = IdentityResult.Success;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] Error Deleting User");
				result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
				_unitOfWork.Rollback();
			}

			return result;

		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			//const string sql = "select * users where id = @id";

			int id = int.Parse(userId);

			try
			{
				var result = await _dbContext.GetAsync<User>(id);
				return result;

			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] FindByIdAsync failed");
				throw;
			}
		}

		public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			const string sql = "select * from " + Constants.Database.TableNames.Users + " where normalized_user_name = @name";

			try
			{
				return await _dbContext.QueryFirstOrDefaultAsync<User>(sql, new { name = normalizedUserName }, cancellationToken : cancellationToken);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] FindByNameAsync failed trying to find ({normalizedUserName}");
				throw;
			}
		}

		public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.NormalizedUserName);
		}

		public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Id.ToString());
		}
		public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.UserName);
		}
		public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
		{
			user.NormalizedUserName = normalizedName;
			return Task.CompletedTask;
		}
		public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
		{
			user.UserName = userName;
			return Task.CompletedTask;
		}

		public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
		{
			IdentityResult result = null;
			try
			{
				const string deleteClaimsSQL = "delete from " + Constants.Database.TableNames.UserClaims + " where user_id = @userId";
				const string insertClaimsSQL = "insert into " + Constants.Database.TableNames.UserClaims + " (user_id, claim_type, claim_value) values(@userId, @claimType, @claimValue)";
				//brute force, just delete all existing claims and add the ones left back in
				if (user.Claims?.Count > 0)
				{
					await _dbContext.ExecuteAsync(deleteClaimsSQL, new { userId = user.Id }, cancellationToken: cancellationToken);
					await _dbContext.ExecuteAsync(insertClaimsSQL,
						user.Claims.Select(x => new
						{
							userId = user.Id,
							claimType = x.Type,
							claimValue = x.Value
						}), cancellationToken: cancellationToken);
				}
				const string deleteLoginsSQL = "delete from " + Constants.Database.TableNames.UserLogins + " where user_id = @userId";
				const string insertLoginsSQL = "insert into " + Constants.Database.TableNames.UserLogins + " (user_id, login_provider, provider_key, provider_display_name) " +
					"values (@userId, @LoginProvider, @ProviderKey, @ProviderDisplayName)";

				if (user.Logins?.Count > 0)
				{
					await _dbContext.ExecuteAsync(deleteLoginsSQL, new { userId = user.Id }, cancellationToken: cancellationToken);
					await _dbContext.ExecuteAsync(insertLoginsSQL,
						user.Logins.Select(x => new
						{
							userId = user.Id,
							x.LoginProvider,
							x.ProviderKey,
							x.ProviderDisplayName
						}), cancellationToken: cancellationToken);
				}

				if (user.Roles?.Count > 1)
				{
					const string deleteRolesSQL = "delete from " + Constants.Database.TableNames.UserRoles + " where user_id = @userId";
					const string insertRolesSQL = "insert into " + Constants.Database.TableNames.UserRoles + " (user_id, role_id) " +
													"values (@userId, @roleId";
					await _dbContext.ExecuteAsync(deleteRolesSQL, new { userId = user.Id }, cancellationToken: cancellationToken);
					await _dbContext.ExecuteAsync(insertRolesSQL,
						user.Roles.Select(x => new
						{
							userId = user.Id,
							roleId = x.RoleId
						}), cancellationToken: cancellationToken);
				}

				if (user.Tokens?.Count > 0)
				{
					const string deleteTokensSQL = "delete from " + Constants.Database.TableNames.UserTokens + " where user_id = @userId";
					const string insertTokensSQL = "insert into " + Constants.Database.TableNames.UserTokens + " (user_Id, login_provider, name, value) " +
													"values (@UserId, @LoginProvider, @Name, @Value)";
					await _dbContext.ExecuteAsync(deleteTokensSQL, new { userId = user.Id }, cancellationToken: cancellationToken);
					await _dbContext.ExecuteAsync(insertTokensSQL,
						user.Tokens.Select(x => new
						{
							x.UserId,
							x.LoginProvider,
							x.Name,
							x.Value
						}), cancellationToken: cancellationToken);
				}

				const string updateSql = "UPDATE " + Constants.Database.TableNames.Users + " " +
					"SET user_name = @UserName, normalized_user_name = @NormalizedUserName, email = @Email, normalized_email = @NormalizedEmail, email_confirmed = @EmailConfirmed, " +
						"password_hash = @PasswordHash, security_stamp = @SecurityStamp, concurrency_stamp = @ConcurrencyStamp, phone_number = @PhoneNumber, " +
						"phone_number_confirmed = @PhoneNumberConfirmed, two_factor_enabled = @TwoFactorEnabled, lockout_end = @LockoutEnd, lockout_enabled = @LockoutEnabled, " +
                        "access_failed_count = @AccessFailedCount , is_organisation = @IsOrganisation, account_suspended = @AccountSuspended, " +
						"refresh_token = @RefreshToken, refresh_token_expiry = @RefreshTokenExpiryTime  \n" +
					"WHERE id = @Id;";

				//await _dbContext.UpdateAsync<User>(user); //doesn't work as it doesn't use the column mappings

				await _dbContext.ExecuteAsync(updateSql,
					new
					{
						user.UserName,
						user.NormalizedUserName,
						user.Email,
						user.NormalizedEmail,
						user.EmailConfirmed,
						user.PasswordHash,
						user.SecurityStamp,
						user.ConcurrencyStamp,
						user.PhoneNumber,
						user.PhoneNumberConfirmed,
						user.TwoFactorEnabled,
						user.LockoutEnd,
						user.LockoutEnabled,
						user.AccessFailedCount,
						user.Id,
						user.IsOrganisation,
						user.AccountSuspended,
						user.RefreshToken,
						user.RefreshTokenExpiryTime
					}, cancellationToken: cancellationToken);

				_unitOfWork.Commit();
				result = IdentityResult.Success;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] UpdateAsync failed"); 
				IdentityError[] errors = { new IdentityError() { Description = ex.Message } };
				result = IdentityResult.Failed(errors);
				_unitOfWork.Rollback();
			}

			return result;
		}

		#endregion UserStore

		#region IUserPasswordStore<User>
		public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash);
		}


		public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash != null);
		}


		public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
		{
			user.PasswordHash = passwordHash;
			return Task.CompletedTask;
		}

		#endregion PasswordStore

		#region IUserEmailStore<User>

		public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
		{
			user.Email = email;
			return Task.CompletedTask;
		}

		public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.EmailConfirmed);
		}

		public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
		{
			user.EmailConfirmed = confirmed;
			return Task.CompletedTask;
		}

		public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			const string sql = "select * from " + Constants.Database.TableNames.Users + " where normalized_email = @normalizedEmail";

			try
			{
				var result = await _dbContext.QueryFirstOrDefaultAsync<User>(sql, new { normalizedEmail }, cancellationToken: cancellationToken); ;
				return result;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] FindByEmailAsync failed while finding {normalizedEmail}");
				throw;
			}
		}

		public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.NormalizedEmail);
		}

		public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
		{
			user.NormalizedEmail = normalizedEmail;
			return Task.CompletedTask;
		}


		#endregion EmailStore

		#region IUserLoginStore<User>

		public async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
		{
			var logins = await GetLoginsAsync(user, cancellationToken);
			var foundLogin = logins.SingleOrDefault(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
			if (foundLogin == null)
			{
				const string createSql = "INSERT INTO " + Constants.Database.TableNames.UserLogins + " " +
					"(user_id, login_provider, provider_key, provider_display_name) " +
					"VALUES (@UserId,@LoginProvider,@ProviderKey, @ProviderDisplayName)";
				try
				{


					var newLogin = new UserLogin()
					{
						UserId = user.Id,
						LoginProvider = login.LoginProvider,
						ProviderKey = login.ProviderKey,
						ProviderDisplayName = login.ProviderDisplayName
					};

					await _dbContext.ExecuteAsync(createSql, newLogin, cancellationToken: cancellationToken);
					_unitOfWork.Commit();
				}
				catch (Exception ex)
				{
					_logger.Error(ex, "[UserStore] AddLoginAsync failed while adding Login {login.ProviderDisplayName} for user {user.UserName}");
					_unitOfWork.Rollback();
					//Log Error
					throw;
				}
			}
		}

		public async Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			const string sql = "delete from " + Constants.Database.TableNames.UserLogins + " where login_provider = @loginProvider and provider_key = @providerKey and user_id = @userId";

			try
			{
				await _dbContext.ExecuteAsync(sql, new { userId = user.Id, loginProvider, providerKey }, cancellationToken: cancellationToken);
				_unitOfWork.Commit();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] RemoveLoginAsync failed while removing Login {loginProvider} from user {user.UserName}");
				_unitOfWork.Rollback();
				//Log Error
				throw;
			}
		}

		public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
		{

			const string sql = "select * from " + Constants.Database.TableNames.UserLogins + " where user_id = @userId";

			try
			{
				var result = await _dbContext.QueryAsync<UserLogin>(sql, new { userId = user.Id }, cancellationToken: cancellationToken);
				return result.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] GetLoginsAsync failed for user {user.UserName}");
				throw;
			}
		}

		public async Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
		{
			const string findIdSql = "select user_id from " + Constants.Database.TableNames.UserLogins + " where login_provider = @loginProvider and provider_key = @providerKey";
			try
			{
				var userId = await _dbContext.QuerySingleOrDefaultAsync<int?>(findIdSql, new { loginProvider, providerKey }, cancellationToken: cancellationToken);
				if (userId.HasValue)
					return await _dbContext.GetAsync<User>(userId.Value);
				return null;
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] FindByLoginAsync failed for login {loginProvider}");
				throw; ;
			}
		}

		#endregion LoginStore

		#region IUserTwoFactorStore<ApplicationUser> Implementation
		public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.TwoFactorEnabled = enabled;
			return Task.CompletedTask;
		}

		public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.TwoFactorEnabled);
		}
		#endregion IUserTwoFactorStore<ApplicationUser> Implementation

		#region IUserSecurityStampStore<User>
		public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.SecurityStamp = stamp;
			return Task.FromResult<object>(null);
		}

		public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(user.SecurityStamp);
		}
		#endregion

		#region IUserClaimStore<User>
		public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (user.Claims == null)
			{
				const string getUserClaimsSql = "select * from " + Constants.Database.TableNames.UserClaims + " where user_id = @userId";

				try
				{
					var userClaims = await _dbContext.QueryAsync<UserClaim>(getUserClaimsSql, new { userId = user.Id }, cancellationToken : cancellationToken);

					user.Claims = userClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

				}
				catch (Exception ex)
				{
					_logger.Error(ex, "[UserStore] GetClaimsAsync failed for user {user.UserName}");
					throw;
				}
			}

			return user.Claims;
		}

		public async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.Claims = user.Claims ?? (await GetClaimsAsync(user, cancellationToken)).ToList();

			foreach (var claim in claims)
			{
				var foundClaim = user.Claims.FirstOrDefault(x => x.Type == claim.Type);

				if (foundClaim != null)
				{
					user.Claims.Remove(foundClaim);
					user.Claims.Add(claim);
				}
				else
				{
					user.Claims.Add(claim);
				}
			}
		}

		public async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.Claims = user.Claims ?? (await GetClaimsAsync(user, cancellationToken)).ToList();
			var foundClaim = user.Claims.FirstOrDefault(x => x.Type == claim.Type && x.Value == claim.Value);

			if (foundClaim != null)
			{
				foundClaim = newClaim;
			}
			else
			{
				user.Claims.Add(newClaim);
			}
		}

		public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.Claims = user.Claims ?? (await GetClaimsAsync(user, cancellationToken)).ToList();

			foreach (var claim in claims)
			{
				user.Claims.Remove(claim);
			}
		}

		public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			string sql = $"select * from {Constants.Database.TableNames.Users}  as u " +
									$"inner join {Constants.Database.TableNames.UserClaims}  as uc on u.id = uc.user_id " +
									$" where uc.claim_type = @ClaimType and uc.claim_value = @ClaimValue";

			try
			{
				return (await _dbContext.QueryAsync<User>(sql, new
				{
					ClaimType = claim.Type,
					ClaimValue = claim.Value
				}, cancellationToken: cancellationToken)).ToList();

			}
			catch (Exception ex)
			{
				_logger.Error(ex, "[UserStore] GetUsersForClaimAsync failed for clain {claim.Type}");
				throw;
			}

		}


		#endregion

		#region IUserLockoutStore<User>
		public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(user.LockoutEnd);
		}

		public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.LockoutEnd = lockoutEnd?.UtcDateTime;
			return Task.CompletedTask;
		}

		public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.AccessFailedCount++;
			return Task.FromResult(user.AccessFailedCount);
		}

		public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.AccessFailedCount = 0;
			return Task.CompletedTask;
		}

		public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(user.AccessFailedCount);
		}

		public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(user.LockoutEnabled);
		}

		public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.LockoutEnabled = enabled;
			return Task.CompletedTask;
		}


		#endregion

		#region IUserRoleStore<User>

		private async Task<IEnumerable<UserRole>> GetUserRolesAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
            try
            {
                const string getRolesSql = "select * from " + Constants.Database.TableNames.UserRoles + " where user_id = @userId";

                return await _dbContext.QueryAsync<UserRole>(getRolesSql, new { userId = user.Id }, cancellationToken : cancellationToken);

            }
            catch //(Exception ex)
            {
                //Log Error
                throw;
            }
        }

		public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			Role role;

			try
			{
				const string findRoleSql = "select * from " + Constants.Database.TableNames.Roles + " where normalized_name = @roleName";

				role = await _dbContext.QuerySingleOrDefaultAsync<Role>(findRoleSql, new { roleName }, cancellationToken : cancellationToken);

			}
			catch //(Exception ex)
			{
				//Log Error
				throw;
			}

			if (role == null)
				throw new ArgumentException("Invalid rolename");

			user.Roles = user.Roles ?? (await GetUserRolesAsync(user, cancellationToken)).ToList();

			//if (user.Roles == null)
			//{
			//	using (var context = _contextFactory.CreateDbContext())
			//	{

			//		try
			//		{
			//			const string getRolesSql = "select * from " + Constants.Database.TableNames.UserRoles + " where user_id = @userId";

			//			user.Roles = (await _dbContext.QueryAsync<UserRole>(getRolesSql, new { userId = user.Id })).ToList();

			//		}
			//		catch //(Exception ex)
			//		{
			//			//Log Error
			//			throw;
			//		}
			//	}
			//}

			if (await IsInRoleAsync(user, roleName, cancellationToken))
				return;

			//user.Roles.Add(new UserRole
			//{
			//	UserId = user.Id,
			//	//RoleName = roleName,
			//	RoleId = role.Id
			//});

			const string insertSql = "insert into " + Constants.Database.TableNames.UserRoles + " values (@userId, @roleId)";

			await _dbContext.ExecuteAsync(insertSql, new { userId = user.Id, roleId = role.Id }, cancellationToken: cancellationToken);
			_unitOfWork.Commit();
		}

		public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			user.Roles = user.Roles ?? (await GetUserRolesAsync(user, cancellationToken)).ToList();

			var role = await _roleStore.FindByNameAsync(roleName, cancellationToken);
			if (role != null)
			{
				var userRole = user.Roles.SingleOrDefault(x => x.RoleId == role.Id);

				if (userRole != null)
				{
					user.Roles.Remove(userRole);
				}

			}
		}
		public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

            try
            {
                const string getUserRoleNamesSql = "select r.name from " + Constants.Database.TableNames.Roles + " r inner join " +
                    Constants.Database.TableNames.UserRoles + " u ON u.role_id = r.id where u.user_id = @userId";

                var result = (await _dbContext.QueryAsync<string>(getUserRoleNamesSql, new { userId = user.Id }, cancellationToken: cancellationToken)).ToList();
                return result;
            }
            catch //(Exception ex)
            {
                //Log Error
                throw;
            }
        }

		public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.Roles = user.Roles ?? (await GetUserRolesAsync(user, cancellationToken)).ToList();

			var role = await _roleStore.FindByNameAsync(roleName.ToUpper(), cancellationToken);
			if (role == null)
				return false;

			return user.Roles.Any(x => x.RoleId == role.Id);
		}

		public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
            try
            {
                const string getUsersInRoleSql = "select * from " + Constants.Database.TableNames.Users + " as u " +
                                           "inner join " + Constants.Database.TableNames.UserRoles + " as ur on u.id = ur.user_id " +
                                           "inner join " + Constants.Database.TableNames.Roles + " as r on ur.role_id = r.id " +
                                            "where r.name = @RoleName";

                return (await _dbContext.QueryAsync<User>(getUsersInRoleSql, new { RoleName = roleName }, cancellationToken: cancellationToken)).ToList();

            }
            catch //(Exception ex)
            {
                //Log Error
                throw;
            }
        }

		#endregion

		#region IUserAuthenticationTokenStore<User>

		private async Task<IEnumerable<UserToken>> GetTokensForUserAsync(int userId)
		{
            try
            {
                const string findTokensSql = "select * from " + Constants.Database.TableNames.UserTokens + " where user_id = @userId";

                return await _dbContext.QueryAsync<UserToken>(findTokensSql, new { userId });

            }
            catch //(Exception ex)
            {
                //Log Error
                throw;
            }
        }

		//private async Task<UserToken> FindTokenAsync(int userId, string loginProvider, string name, CancellationToken cancellationToken)
		//{
		//	const string findTokensSql = $@"select * from {Constants.Database.TableNames.UserTokens} where 
		//								  user_id = @userId 
		//								  and login_provider = @loginProvider
		//							      and name = @name";

  //          return await _dbContext.QueryFirstOrDefaultAsync<UserToken>(findTokensSql, new { userId, loginProvider, name }, cancellationToken : cancellationToken);

  //      }



        public async Task SetTokenAsync(User user, string loginProvider, string name, string value, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

        //    var token = await FindTokenAsync(user.Id, loginProvider, name, cancellationToken).ConfigureAwait(false);
            user.Tokens = user.Tokens ?? (await GetTokensForUserAsync(user.Id)).ToList();
			var token = user.Tokens.FirstOrDefault(token => token.LoginProvider == loginProvider && token.Name == name );
            if (token == null)
            {
                //user.Tokens = user.Tokens ?? (await GetTokensForUserAsync(user.Id)).ToList();

                user.Tokens.Add(new UserToken
                {
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value
                });
            }
            else
            {
                token.Value = value;
            }
		}

		public async Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.Tokens = user.Tokens ?? (await GetTokensForUserAsync(user.Id)).ToList();
			var token = user.Tokens.SingleOrDefault(x => x.LoginProvider == loginProvider && x.Name == name);
			user.Tokens.Remove(token);
		}

		public async Task<string> GetTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.Tokens = user.Tokens ?? (await GetTokensForUserAsync(user.Id)).ToList();
			return user.Tokens.SingleOrDefault(x => x.LoginProvider == loginProvider && x.Name == name)?.Value;
		}

		#endregion

		#region IUserPhoneNumberStore<User> Implementation
		public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.PhoneNumber = phoneNumber;
			return Task.CompletedTask;
		}

		public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(user.PhoneNumber);
		}

		public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(user.PhoneNumberConfirmed);
		}

		public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			user.PhoneNumberConfirmed = confirmed;
			return Task.CompletedTask;
		}
		#endregion IUserPhoneNumberStore<ApplicationUser> Implementation

		#region IUserAuthenticatorKeyStore<User>

		private const string AuthenticatorStoreLoginProvider = "[AspNetAuthenticatorStore]";
		private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
		private const string RecoveryCodeTokenName = "RecoveryCodes";

		public async Task<string> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
		{
			//return Task.FromResult(user.TwoFactorAuthenticatorKey);
			return await GetTokenAsync(user, AuthenticatorStoreLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
		}

		public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
		{
			// user.TwoFactorAuthenticatorKey = key;
			return SetTokenAsync(user, AuthenticatorStoreLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
			//return Task.CompletedTask;
		}

		#endregion

		#region IUserTwoFactorRecoveryCodeStore<User>
		public Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
		{
			var mergedCodes = string.Join(";", recoveryCodes);
			return SetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
		}

		public Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
		{
			var mergedCodes = GetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, cancellationToken).Result;
			var splitCodes = (mergedCodes ?? string.Empty).Split(';');
			if (splitCodes.Contains(code))
			{
				var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
				ReplaceCodesAsync(user, updatedCodes, cancellationToken);
				return Task.FromResult(true);
			}
			return Task.FromResult(false);
		}

		public Task<int> CountCodesAsync(User user, CancellationToken cancellationToken)
		{
			var mergedCodes = GetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, cancellationToken).Result ?? string.Empty;
			if (mergedCodes.Length > 0)
				return Task.FromResult(mergedCodes.Split(';').Length);
			return Task.FromResult(0);
		}
		#endregion


	}
}
