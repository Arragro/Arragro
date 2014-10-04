using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using System.Linq;

namespace Arragro.Common.ServiceBase
{
    public abstract class AuditableService<TRepository, TModel, TKeyType, TUserIdType> : AuditableBusinessRulesBase<TRepository, TModel, TKeyType, TUserIdType>
        where TModel : class, IAuditable<TUserIdType>
        where TRepository : IRepository<TModel, TKeyType>
    {
        public AuditableService(TRepository repository)
            : base(repository)
        {
        }

        public TModel Find(TKeyType id)
        {
            return Repository.Find(id);
        }

        protected abstract void ValidateModelRules(TModel model);

        public abstract TModel InsertOrUpdate(TModel model, TUserIdType userId);

        public void ValidateModel(TModel model)
        {
            RulesException.ErrorsForValidationResults(ValidateModelProperties(model));
            ValidateModelRules(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public TModel ValidateAndInsertOrUpdate(TModel model, TUserIdType userId)
        {
            ValidateModel(model);
            return InsertOrUpdate(model, userId);
        }
    }
}