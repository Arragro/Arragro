using Arragro.Common.Repository;
using System;

namespace Arragro.Common.BusinessRules
{
    public class AuditableBusinessRulesBase<TRepository, TModel, TKeyType, TUserIdType> : BusinessRulesBase<TRepository, TModel, TKeyType>
        where TModel : class, IAuditable<TUserIdType>
        where TRepository : IRepository<TModel, TKeyType>
    {
        protected new TRepository Repository { get { return base.Repository; } }

        public AuditableBusinessRulesBase(TRepository repository) : base(repository) { }

        protected void AddOrUpdateAudit(TModel entity, TUserIdType userId, bool add)
        {
            entity.ModifiedBy = userId;
            if (add)
            {
                entity.CreatedBy = userId;
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = entity.CreatedDate;
            }
            else
            {
                entity.ModifiedDate = DateTime.Now;
            }
        }
    }
}