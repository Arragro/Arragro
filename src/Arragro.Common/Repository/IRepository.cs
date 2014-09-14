using System.Linq;

namespace Arragro.Common.Repository
{
    public interface IRepository<TModel, TKeyType> where TModel : class
    {
        void TurnOnOffLazyLoading(bool on = true);
        TModel Find(TKeyType id);
        TModel Delete(TKeyType id);
        IQueryable<TModel> All();
        TModel InsertOrUpdate(TModel model, bool add);
        int SaveChanges();
    }
}