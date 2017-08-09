using Arragro.Common.Repository;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.EntityFrameworkCore.Interfaces
{
    public interface IDbContextRepositoryBase<TEntity> :
        IRepository<TEntity> where TEntity : class
    {
        IBaseContext BaseContext { get; }
        
        IQueryable<TEntity> AllNoTracking(Expression<Func<TEntity, bool>> whereClause);
    }
}
