using DPMGallery.Data;
using System;
using System.Linq;

namespace DPMGallery.Repositories
{
    public class RepositoryBase
    {
        protected readonly IDbContext _dbContext;

        public RepositoryBase(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected string PagingSQL => " offset @skip limit @take";
        //        {
        //leaving this here in case we need to change db's in the future
        //if (_dbContext.DatabaseType == DatabaseType.PostgreSQL)
        //{
        //   return " offset @skip limit @take";
        //}
        //else
        //{
        //    //sql server 2012+  must be used with order by!
        //    return " offset @skip rows fetch next @take rows only";
        //}
        //      }

        protected IDbContext Context => _dbContext;
    }
}
