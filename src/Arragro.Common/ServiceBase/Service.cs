using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using System.Linq;

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

        protected abstract void ValidateModelRules(TModel model);

        public abstract TModel InsertOrUpdate(TModel model);

        public void ValidateModel(TModel model)
        {
            RulesException.ErrorsForValidationResults(ValidateModelProperties(model));
            ValidateModelRules(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public TModel ValidateAndInsertOrUpdate(TModel model)
        {
            ValidateModel(model);
            return InsertOrUpdate(model);
        }
    }
}