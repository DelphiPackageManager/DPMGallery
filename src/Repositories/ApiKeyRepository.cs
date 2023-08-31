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

namespace DPMGallery.Repositories
{
    public class ApiKeyRepository : RepositoryBase
    {
        //using this to ensure we never get the hashed value
        private const string allowedColumns = "id, user_id, name, expires_utc, glob_pattern, package_list, scopes, revoked";
        private readonly ILogger _logger;
        public ApiKeyRepository(IDbContext dbContext, ILogger logger) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<ApiKey> GetApiKeyByKey(string unhashed)
        {
            string hashed = unhashed.GetHashSha256();
            string q = $"select {allowedColumns} from {T.ApiKey} where key_hashed = @hashed";
            return await Context.QueryFirstOrDefaultAsync<ApiKey>(q, new { hashed });
        }

        public async Task<ApiKey> GetApiKeyById(int id, CancellationToken cancellationToken)
        {
            string q = $"select {allowedColumns} from {T.ApiKey} where id = @id";
            return await Context.QueryFirstOrDefaultAsync<ApiKey>(q, new { id }, cancellationToken : cancellationToken);
        }


        public async Task<PageableEnumeration<ApiKey>> GetApiKeys(Paging paging, CancellationToken cancellationToken)
        {
            string cq = $"select count(*) from {T.ApiKey}";
            string q = $"select {allowedColumns} from {T.ApiKey} order by id " + PagingSQL; //sql server paging requires order by

            int totalCount = await Context.ExecuteScalarAsync<int>(cq, cancellationToken : cancellationToken);

            List<ApiKey> keys;

            if (totalCount > 0)
            {
                keys = (await Context.QueryAsync<ApiKey>(q, new { skip = paging.Skip, take = paging.Take })).ToList();
            }
            else
            {
                keys = new List<ApiKey>();
            }

            return new PageableEnumeration<ApiKey>(keys, totalCount);

        }

        //not doing paging for now.
        public async Task<IEnumerable<ApiKey>> GetApiKeysForUser(int userId, CancellationToken cancellationToken)
        {

            //            string cq = $"select count(*) from {T.ApiKey} where user_id = @userId";
            string q = $"select {allowedColumns} from {T.ApiKey} where user_id = @userId order by revoked asc, expires_utc desc  ";// + PagingSQL;
  //          int totalCount = await Context.ExecuteScalarAsync<int>(cq, new { userId });

            //List<ApiKey> keys;

            //if (totalCount > 0)
            //{
                return  (await Context.QueryAsync<ApiKey>(q, new { userId /*, skip = paging.Skip, take = paging.Take */}, cancellationToken: cancellationToken)).ToList();
            //}
            //else
            //{
            //    keys = new List<ApiKey>();
            //}

            //return new PageableEnumeration<ApiKey>(keys, totalCount);

        }

        public async Task<ApiKey> Insert(ApiKey key)
        {
            key.KeyHashed = key.Key.GetHashSha256();

            const string insertSql = "INSERT INTO " + Constants.Database.TableNames.ApiKey + " " +
                "VALUES(@KeyHashed, @UserId, @ExpiresUTC, @GlobPattern, @Packages, @Scopes ) RETURNING id";

            try
            {
                key.Id = await Context.ExecuteScalarAsync<int>(insertSql, new
                {
                    key.KeyHashed,
                    key.UserId,
                    key.ExpiresUTC,
                    key.GlobPattern,
                    key.Packages,
                    key.Scopes,
                });
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "[ApiKeyRepository] AddNewKey failed for User: {key.UserId}  Key Name : {key.Name}");
                throw;
            }

            return key;

        }

        public async Task<ApiKey> Update(ApiKey key)
        {
            const string updateSql = "UPDATE " + T.ApiKey + " " +
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

        public async Task Delete(ApiKey key)
        {
            const string deleteSql = "DELETE FROM " + T.ApiKey + " WHERE id = @id";
            try
            {
                await Context.ExecuteAsync(deleteSql, new { key.Id });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[ApiKeyRepository] DeleteKey failed for Key {key.Id}");
                throw;
            }
        }
    }
}

