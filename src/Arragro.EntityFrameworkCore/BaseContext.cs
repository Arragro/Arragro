using Arragro.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arragro.EntityFrameworkCore
{
    public class BaseContext : DbContext, IBaseContext
    {
        public BaseContext(DbContextOptions options, QueryTrackingBehavior queryTrackingBehaviour = QueryTrackingBehavior.NoTracking) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = queryTrackingBehaviour;
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        protected new void Dispose()
        {
            base.Dispose();
        }
    }
}
