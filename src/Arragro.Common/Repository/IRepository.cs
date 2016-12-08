using System.Linq;

namespace Arragro.Common.Repository
{
    public interface IRepository<TModel, TKeyType> where TModel : class
    {
        TModel Find(TKeyType id);
        TModel Delete(TKeyType id);
        IQueryable<TModel> All();
        IQueryable<TModel> AllNoTracking();
        TModel InsertOrUpdate(TModel model, bool add);
        int SaveChanges();
    }
}