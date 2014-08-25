using Arragro.Common.Repository;

namespace Arragro.Common.BusinessRules
{
    public class BusinessRulesBase<TRepository, TModel, TKeyType>
        where TModel : class
        where TRepository : IRepository<TModel, TKeyType>
    {
        protected readonly TRepository RepositoryBase;

        public BusinessRulesBase(TRepository repositoryBase)
        {
            RepositoryBase = repositoryBase;
            RulesException = new RulesException<TModel>();
        }

        public readonly RulesException<TModel> RulesException;
    }
}
