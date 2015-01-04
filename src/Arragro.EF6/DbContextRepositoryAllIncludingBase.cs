using Arragro.Common.Repository;
using Arragro.EF6.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.EF6
{
    public class DbContextRepositoryAllIncludingBase<TEntity, TKey> : 
        DbContextRepositoryBase<TEntity, TKey>, 
        IDbContextAllIncludingRepositoryBase<TEntity, TKey>,
        IRepository<TEntity, TKey> where TEntity : class
    {
        public DbContextRepositoryAllIncludingBase(IBaseContext baseContext) : base(baseContext) { }

        public virtual IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual IQueryable<TEntity> AllIncludingNoTracking(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return AllIncluding(whereClause, includeProperties).AsNoTracking();
        }

        public virtual IQueryable<TEntity> AllIncludingNoTracking(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return AllIncluding(includeProperties).AsNoTracking();
        }

        public virtual IQueryable<TEntity> AllIncluding(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet.Where(whereClause);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public new void InsertOrUpdate(TEntity model, bool add)
        {
            if (add)
            {
                DbSet.Add(model);
            }
            else
            {
                BaseContext.SetModified(model);
            }
        }
    }
}
