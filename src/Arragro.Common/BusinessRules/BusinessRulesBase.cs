using Arragro.Common.Repository;
using Arragro.Common.RulesExceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arragro.Common.BusinessRules
{
    public class BusinessRulesBase<TRepository, TModel>
        where TModel : class
        where TRepository : IRepository<TModel>
    {
        protected readonly TRepository Repository;

        public BusinessRulesBase(TRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repositoryBase");
            Repository = repository;
            RulesException = new RulesException<TModel>();
        }

        protected IList<ValidationResult> ValidateModelProperties(TModel model)
        {
            return model.ValidateModelProperties();
        }

        public readonly RulesException<TModel> RulesException;

        public int SaveChanges()
        {
            return Repository.SaveChanges();
        }
    }
}