using Arragro.Common.Repository;
using Arragro.EF6.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.EF6
{
    public class DbContextRepositoryBase<TEntity, TKey> : 
        IDbContextRepositoryBase<TEntity, TKey>,
        IRepository<TEntity, TKey> where TEntity : class
    {
        private readonly IBaseContext _baseContext;

        public IBaseContext BaseContext { get { return _baseContext; } }

        public DbContextRepositoryBase(IBaseContext baseContext)
        {
            _baseContext = baseContext;
        }

        protected IDbSet<TEntity> DbSet
        {
            get { return _baseContext.Set<TEntity>(); }
        }

        public void TurnOnOffLazyLoading(bool enabled = true)
        {
            _baseContext.Configuration.LazyLoadingEnabled = enabled;
        }

        public TEntity Find(TKey id)
        {
            return DbSet.Find(id);
        }

        public TEntity Delete(TKey id)
        {
            return DbSet.Remove(Find(id));
        }

        public virtual IQueryable<TEntity> All()
        {
            return DbSet;
        }

        public virtual IQueryable<TEntity> AllNoTracking()
        {
            return DbSet.AsNoTracking();
        }

        public virtual IQueryable<TEntity> AllNoTracking(Expression<Func<TEntity, bool>> whereClause)
        {
            return DbSet.Where(whereClause).AsNoTracking();
        }

        public TEntity InsertOrUpdate(TEntity model, bool add)
        {
            if (add)
            {
                return DbSet.Add(model);
            }
            else
            {
                _baseContext.SetModified(model);
                return model;
            }
        }

        public int SaveChanges()
        {
            try
            {
                return _baseContext.SaveChanges();
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException;
                throw;
            }
        }

        public void Dispose()
        {
            _baseContext.Dispose();
        }
    }
}
