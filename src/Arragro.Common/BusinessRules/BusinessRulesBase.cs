using Arragro.Common.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        
        public IList<ValidationResult> ValidateModel(TModel model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        public readonly RulesException<TModel> RulesException;

        public int SaveChanges()
        {
            return Repository.SaveChanges();
        }
    }
}
