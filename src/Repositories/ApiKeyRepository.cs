using DPMGallery.Data;
using DPMGallery.Entities;
using DPMGallery.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using T = DPMGallery.Constants.Database.TableNames;
using Serilog;
using System.Data.Common;
using Dapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace DPMGallery.Repositories
{
    public class ApiKeyRepository : RepositoryBase
    {
        //using this to ensure we never get the hashed value
        private const string allowedColumns = "id, user_id, name, expires_utc, glob_pattern, package_list, scopes, revoked";
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ApiKeyRepository(IDbContext dbContext, ILogger logger) : base(dbContext)
        {
            _unitOfWork = dbContext as IUnitOfWork;
            _logger = logger;

        }

        public async Task<ApiKey> GetApiKeyByKey(string unhashed)
        {
            string hashed = unhashed.GetHashSha256();
            string q = $"select {allowedColumns} from {T.ApiKeys} where key_hashed = @hashed";
            return await Context.QueryFirstOrDefaultAsync<ApiKey>(q, new { hashed });
        }

        public async Task<ApiKey> GetApiKeyById(int id, CancellationToken cancellationToken)
        {
            string q = $"select {allowedColumns} from {T.ApiKeys} where id = @id";
            return await Context.QueryFirstOrDefaultAsync<ApiKey>(q, new { id }, cancellationToken : cancellationToken);
        }


        //public async Task<PageableEnumeration<ApiKey>> GetApiKeys(Paging paging, CancellationToken cancellationToken)
        //{
        //    string cq = $"select count(*) from {T.ApiKeys}";
        //    string q = $"select {allowedColumns} from {T.ApiKeys} order by id " + PagingSQL; //sql server paging requires order by

        //    int totalCount = await Context.ExecuteScalarAsync<int>(cq, cancellationToken : cancellationToken);

        //    List<ApiKey> keys;

        //    if (totalCount > 0)
        //    {
        //        keys = (await Context.QueryAsync<ApiKey>(q, new { skip = paging.Skip, take = paging.Take })).ToList();
        //    }
        //    else
        //    {
        //        keys = new List<ApiKey>();
        //    }

        //    return new PageableEnumeration<ApiKey>(keys, totalCount);

        //}

        //not doing paging for now.
        //      public async Task<PagedList<ApiKey>> GetApiKeysForUser(int userId, Paging paging, CancellationToken cancellationToken)
        //      {

        //          //            string cq = $"select count(*) from {T.ApiKeys} where user_id = @userId";
        //          string q = $"select {allowedColumns} from {T.ApiKeys} where user_id = @userId order by revoked asc, expires_utc desc  ";// + PagingSQL;
        ////          int totalCount = await Context.ExecuteScalarAsync<int>(cq, new { userId });

        //          //List<ApiKey> keys;

        //          //if (totalCount > 0)
        //          //{
        //              return  (await Context.QueryAsync<ApiKey>(q, new { userId /*, skip = paging.Skip, take = paging.Take */}, cancellationToken: cancellationToken)).ToList();
        //          //}
        //          //else
        //          //{
        //          //    keys = new List<ApiKey>();
        //          //}

        //          //return new PageableEnumeration<ApiKey>(keys, totalCount);

        //      }

        private static string PagingSortToDbColumn(string sort)
        {
            string lcSort = sort?.Trim().ToLowerInvariant() ?? string.Empty;

            string columnName = lcSort switch
            {
                "id" => "id",
                "name" => "name",
                "expiresutc" => "expires_utc",
                _ => "name"
            };
            return columnName;
        }
        private async Task<List<ApiKey>> FetchApiKeysForUser(int userId, Paging paging, CancellationToken cancellationToken)
        {
            string sortField = PagingSortToDbColumn(paging.Sort);
            string sortDirection = paging.SortDirection == SortDirection.Default ? "ASC" : paging.SortDirection.ToString().ToUpper();
            string orderByClause = $"ORDER BY {sortField} {sortDirection}";

            string fetchSql =
                    $"""
					SELECT {allowedColumns} FROM {T.ApiKeys}
					WHERE user_id = @userId
					{orderByClause} 
					{Constants.Database.PagingSQL}
					""";
            IEnumerable<ApiKey> query = await Context.QueryAsync<ApiKey>(fetchSql, new { userId, skip = paging.Skip, take = paging.Take, filter = paging.Filter }, cancellationToken: cancellationToken);

            List<ApiKey> keys = query.ToList();

            return keys;
        }

        public async Task<List<string>> GetPackagesOwnedByUser(int userId, CancellationToken cancellationToken)
        {
            string fetchSql =
                    @$"select p.packageid from {T.PackageOwner} o
                        left join {T.Package} p on p.id = o.package_id
                        where
                        o.owner_id in (
                        select @userId as id
                        union 
                        select u.id from {T.Users} u 
                            left join {T.OrganisationMembers} m ON m.org_id = u.id 
                            where 
                            m.member_id = @userId)
                     order by p.packageid";
            IEnumerable<string> query = await Context.QueryAsync<string>(fetchSql, new { userId }, cancellationToken: cancellationToken);

            List<string> result = query.ToList();

            return result;
        }



        public async Task<PagedList<ApiKey>> GetApiKeysForUser(int userId, Paging paging, CancellationToken cancellationToken)
        {

            string countSql = $"SELECT COUNT(*) FROM {T.ApiKeys} WHERE user_id = @userId";

            int totalCount = await Context.ExecuteScalarAsync<int>(countSql, new { userId });

            List<ApiKey> keys;
            if (totalCount == 0)
                keys = [];
            else
            {
                paging.EnsureValidPage(totalCount);
                keys = await FetchApiKeysForUser(userId, paging, cancellationToken);
            }

            //if (Context is IUnitOfWork unitOfWork)
            //    await unitOfWork.CommitAsync();

            return new PagedList<ApiKey>(keys, totalCount, paging);

        }


        public async Task<bool> ApiKeyNameExists(string name)
        {
            string lcName = name.ToLowerInvariant();
            string sql = $"SELECT count(*) FROM {T.ApiKeys} WHERE lower(name) = @name";
            int count = await Context.ExecuteScalarAsync<int>(sql, new { name = lcName });
            return count > 0;
        }


        public async Task<ApiKey> Insert(ApiKey key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key.Key))
				throw new ArgumentException("Key cannot be empty", nameof(key.Key));
        
            key.KeyHashed = key.Key.ToLower().GetHashSha256();

            const string insertSql = $@"INSERT INTO {T.ApiKeys} (user_id, name, key_hashed, expires_utc, glob_pattern, package_list, scopes, revoked, package_owner)
                                      VALUES(@UserId, @Name, @KeyHashed, @ExpiresUTC, @GlobPattern, @Packages, @Scopes, @Revoked, @PackageOwner ) RETURNING id";

            try
            {
                int scopes = (int)key.Scopes;
                var sqlParams = new
                {
                    key.UserId,
                    key.Name,
                    key.KeyHashed,
                    key.ExpiresUTC,
                    key.GlobPattern,
                    key.Packages,
                    Scopes = scopes,
                    Revoked = false,
                    key.PackageOwner
                };

                key.Id = await Context.ExecuteScalarAsync<int>(insertSql, sqlParams, cancellationToken:cancellationToken);
                
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "[ApiKeyRepository] AddNewKey failed for User: {key.UserId}  Key Name : {key.Name}");
                throw;
            }

            return key;

        }

        public async Task<bool> UpdateApiKeyRevoked(int id, bool revoked, CancellationToken cancellationToken = default)
        {
            const string sql = $"UPDATE {T.ApiKeys} SET Revoked = @revoked WHERE id = @id";
            try
            {
                int affectedRows = await Context.ExecuteAsync(sql, new { revoked, id }, cancellationToken: cancellationToken);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[ApiKeyRepository] UpdateApiKeyRevoked failed for Key id : {id}", [id]);
                throw;
            }
        }

        public async Task<bool> UpdateApiKey(int keyId, DateTime? expiresUtc, string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be empty", nameof(key));

            string keyHashed = key.ToLower().GetHashSha256();

            string sql;
            object param;

            //if expiresInDays is null, we don't update it
            if (expiresUtc.HasValue)
            {
                sql = $"UPDATE {T.ApiKeys} SET key_hashed = @keyHashed, expires_utc = @expiresUtc WHERE id = @keyId";
                param = new { keyId, keyHashed, expiresUtc };
            }
            else
            {
                sql = $"UPDATE {T.ApiKeys} SET key_hashed = @keyHashed WHERE id = @keyId";
                param = new { keyId, keyHashed };
            }

            try
            {
                int affectedRows = await Context.ExecuteAsync(sql, param, cancellationToken: cancellationToken);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[ApiKeyRepository] UpdateApiKey failed for Key id : {id}", [keyId]);
                throw;
            }
        }


        public async Task<ApiKey> Update(ApiKey key)
        {
            const string updateSql = "UPDATE " + T.ApiKeys + " " +
                        "SET package_list = @Packages, glob_pattern = @GlobPattern, scopes = @Scopes, expires_utc = @ExpiresUTC " +
                        " WHERE id = @Id";
            try
            {
                await Context.ExecuteAsync(updateSql, new
                {
                    key.Id,
                    key.ExpiresUTC,
                    key.GlobPattern,
                    key.Packages,
                    key.Scopes

                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[ApiKeyRepository] UpdateKey failed for Key Id {key.Id}");
                throw;
            }
            return key;
        }

        public async Task<bool> Delete(int id, CancellationToken cancellationToken = default)
        {
            try
            {

                const string sql = $"DELETE FROM {T.ApiKeys} WHERE id = @id";
                int affectedRows = await Context.ExecuteAsync(sql, new { id }, cancellationToken: cancellationToken);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[ApiKeyRepository] Api key deletion failed for key with id {id}");
                throw;
            }
        }
    }
}