using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;

namespace Arragro.Common.ServiceBase
{
    public abstract class AuditableService<TRepository, TModel, TKeyType, TUserIdType> : AuditableBusinessRulesBase<TRepository, TModel, TKeyType, TUserIdType>
        where TModel : class, IAuditable<TUserIdType>
        where TRepository : IRepository<TModel, TKeyType>
    {
        protected new IRepository<TModel, TKeyType> Repository { get { return base.Repository; } }

        public AuditableService(TRepository repository) : base(repository) { }

        public abstract void EnsureValidModel(TModel model, params object[] relatedModels);
        public abstract TModel InsertOrUpdate(TModel model, TUserIdType userId);
    }
}
