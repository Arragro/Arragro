using System;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.Common.Repository
{
    public interface IRepository<TModel, TKeyType> where TModel : class
    {
        void TurnOnOffLazyLoading(bool on = true);
        TModel Find(TKeyType id);
        void Delete(TKeyType id);
        IQueryable<TModel> All();
        TModel InsertOrUpdate(TModel model, bool add);
        int SaveChanges();
    }
}