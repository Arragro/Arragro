using Arragro.Common.Repository;
using System;

namespace Arragro.Common.BusinessRules
{
    public class BusinessRulesBase<TRepository, TModel, TKeyType>
        where TModel : class
        where TRepository : IRepository<TModel, TKeyType>
    {
        protected readonly TRepository Repository;

        public BusinessRulesBase(TRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repositoryBase");
            Repository = repository;
            RulesException = new RulesException<TModel>();
        }

        public readonly RulesException<TModel> RulesException;
    }
}
