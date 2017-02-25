using Arragro.Common.Helpers;
using Arragro.Common.Repository;
using Arragro.Common.RulesExceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Arragro.Common.BusinessRules
{
    public class BusinessRulesBase<TRepository, TModel> : RulesBase<TModel>
        where TModel : class
        where TRepository : IRepository<TModel>
    {
        protected readonly TRepository Repository;

        public BusinessRulesBase(TRepository repository) : base()
        {
            if (repository == null)
                throw new ArgumentNullException("repositoryBase");
            Repository = repository;
        }

        public int SaveChanges()
        {
            return Repository.SaveChanges();
        }
    }
}