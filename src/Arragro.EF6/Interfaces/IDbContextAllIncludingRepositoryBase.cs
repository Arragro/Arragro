using System;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.EF6.Interfaces
{
    public interface IDbContextAllIncludingRepositoryBase<TEntity, TKey> : 
        IDbContextRepositoryBase<TEntity, TKey> where TEntity : class
    {
        IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> AllIncluding(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
