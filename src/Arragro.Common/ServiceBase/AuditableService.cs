using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using System.Linq;

namespace Arragro.Common.ServiceBase
{
    public abstract class AuditableService<TRepository, TModel, TUserIdType> : AuditableBusinessRulesBase<TRepository, TModel, TUserIdType>
       where TModel : class, IAuditable<TUserIdType>
       where TRepository : IRepository<TModel>
    {
        public AuditableService(TRepository repository)
            : base(repository)
        {
        }

        public TModel Find(params object[] ids)
        {
            return Repository.Find(ids);
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