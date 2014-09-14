using Arragro.Common.Repository;

namespace Arragro.EF6.Interfaces
{
    public interface IDbContextRepositoryBase<TEntity, TKey> : 
        IRepository<TEntity, TKey> where TEntity : class
    {
        IBaseContext BaseContext { get; }
    }
}
