using System;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.EntityFrameworkCore.Interfaces
{
    public interface IDbContextRepositoryAllIncludingBase<TEntity> :
        IDbContextRepositoryBase<TEntity> where TEntity : class
    {
        IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> AllIncluding(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> AllIncludingNoTracking(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> AllIncludingNoTracking(params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
