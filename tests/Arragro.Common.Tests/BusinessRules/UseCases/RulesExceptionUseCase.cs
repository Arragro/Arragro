﻿using Arragro.Common.BusinessRules;
using System.Linq;
using Xunit;

namespace Arragro.Common.Tests.BusinessRules.UseCases
{
    public class RulesExceptionUseCase
    {
        private class ModelFoo
        {
            public int ModelFooId { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public void RulesExceptionUseCaseModelFoo()
        {
            var modelFoo = new ModelFoo
            {
                ModelFooId = 1,
                Name = "Test"
            };

            var rulesException = new RulesException<ModelFoo>();

            // This will apply to the model as whole and should be used for 
            // scenarios where there are multiple issues against another object.
            rulesException.ErrorForModel("There is already a ModelFoo with this Id and Name");
            Assert.Single(rulesException.Errors);

            // Should be used for property issues.
            rulesException.ErrorFor(x => modelFoo.Name, "The Name is not Unique");

            var errorMessage = rulesException.ToString();
            Assert.Equal(2, rulesException.Errors.Count());
        }
    }
}
