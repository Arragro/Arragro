using Arragro.Common.Repository;

namespace Arragro.EntityFrameworkCore.Interfaces
{
    public interface IDbContextRepositoryBase<TEntity> :
        IRepository<TEntity> where TEntity : class
    {
        IBaseContext BaseContext { get; }
    }
}
