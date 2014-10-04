using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;

namespace Arragro.Common.ServiceBase
{
    public abstract class Service<TRepository, TModel, TKeyType> : BusinessRulesBase<TRepository, TModel, TKeyType>
        where TModel : class
        where TRepository : IRepository<TModel, TKeyType>
    {
        public Service(TRepository repository)
            : base(repository)
        {
        }

        public TModel Find(TKeyType id)
        {
            return Repository.Find(id);
        }

        public abstract void EnsureValidModel(TModel model, params object[] relatedModels);

        public abstract TModel InsertOrUpdate(TModel model);

        public TModel ValidateAndInsertOrUpdate(TModel model, params object[] relatedModels)
        {
            EnsureValidModel(model, relatedModels);
            return InsertOrUpdate(model);
        }
    }
}