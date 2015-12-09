using Arragro.EF6.Interfaces;
using System.Data.Entity;

namespace Arragro.EF6
{
    public class BaseContext : DbContext, IBaseContext
    {
        public BaseContext() : base() { }

        public BaseContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
