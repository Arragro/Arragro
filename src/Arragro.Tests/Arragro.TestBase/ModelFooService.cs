using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using Arragro.Common.ServiceBase;
using System;
using System.Linq;

namespace Arragro.TestBase
{
    public class ModelFooService : Service<IRepository<ModelFoo, int>, ModelFoo, int>
    {
        public const string DuplicateName = "There is already a Model Foo with that name in the repository";
        public const string RequiredName = "The Name field is required";
        public const string RangeLengthName = "The Name field must have between 3 and 6 characters";

        public ModelFooService(IRepository<ModelFoo, int> modelFooRepository)
            : base(modelFooRepository)
        {
        }

        /*
         * This function would be implemented further down the chain as
         * BusinessRulesBase provides the structure, not the implementation
         * which would be custom per Model.
         *
         * This would occur on a InsertOrUpdate at the service layer.
         */

        public override void EnsureValidModel(ModelFoo modelFoo, params object[] relatedObjects)
        {
            if (Repository.All()
                    .Where(x => x.Id != modelFoo.Id
                             && x.Name == modelFoo.Name).Any())
                RulesException.ErrorFor(x => x.Name, DuplicateName);

            if (String.IsNullOrEmpty(modelFoo.Name))
                RulesException.ErrorFor(x => x.Name, RequiredName);
            else if (modelFoo.Name.Length < 2 || modelFoo.Name.Length > 6)
                RulesException.ErrorFor(c => c.Name, RangeLengthName);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public override ModelFoo InsertOrUpdate(ModelFoo model)
        {
            model = Repository.InsertOrUpdate(model, model.Id == default(int));
            return model;
        }
    }
}