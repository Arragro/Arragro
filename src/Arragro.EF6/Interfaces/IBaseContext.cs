using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Arragro.EF6.Interfaces
{
    public interface IBaseContext : IDisposable
    {
        DbContextConfiguration Configuration { get; }
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        void SetModified(object entity);
        int SaveChanges();
    }
}
