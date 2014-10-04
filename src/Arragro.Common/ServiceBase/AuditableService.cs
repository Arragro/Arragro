using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;

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

        public abstract void EnsureValidModel(TModel model, params object[] relatedModels);

        public abstract TModel InsertOrUpdate(TModel model, TUserIdType userId);

        public TModel ValidateAndInsertOrUpdate(TModel model, params object[] relatedModels)
        {
            EnsureValidModel(model, relatedModels);
            return InsertOrUpdate(model);
        }
    }
}