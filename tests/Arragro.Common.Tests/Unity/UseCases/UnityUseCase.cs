﻿using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using Arragro.Common.ServiceBase;
using Arragro.TestBase;
using Unity;
using Xunit;

namespace Arragro.Common.Tests.Unity.UseCases
{
    public class UnityUseCase
    {
        // A contract for the unity (ioc) container to use
        private interface IModelFooRepository : IRepository<ModelFoo, int>
        { }

        // An implementation of the contract IModelFooRepository using the InMemoryRepository
        private class InMemoryModelFooRepository : InMemoryRepository<ModelFoo, int>, IModelFooRepository
        { }

        /*
         * An implementation of the contract IModelFooService using the Service base abstract class.
         * All interaction with the Repository should be provided through this Service.
         */

        private class ModelFooService : Service<IModelFooRepository, ModelFoo, int>
        {
            public const string INVALIDLENGTH = "The Name must be less than 4 characters";

            public ModelFooService(IModelFooRepository modelFooRepository)
                : base(modelFooRepository)
            {
            }

            protected override void ValidateModelRules(ModelFoo model)
            {
                if (model.Name.Length > 4)
                    RulesException.ErrorFor(x => x.Name, INVALIDLENGTH);
            }

            public override ModelFoo InsertOrUpdate(ModelFoo model)
            {
                return Repository.InsertOrUpdate(model, model.Id == default(int));
            }
        }

        private IUnityContainer GetInMemoryContainerExample()
        {
            // Instantiate a new unity container
            var unityContainer = new UnityContainer();

            // Register the Interfaces and their Implementations:
            /*
             * The InMemoryModelFooRepository which has minimal implementation code
             * can be switch for a EFModelFooRepository in production, by simply
             * switching the class below for the same interface
             */
            unityContainer.RegisterType<IModelFooRepository, InMemoryModelFooRepository>();
            /*
             * ModelFooService is registered as the unity container will resolve it's
             * constructor upon instantiation which has a dependency on IModelFooRepository.
             * Which is registered above.
             */
            unityContainer.RegisterType<ModelFooService, ModelFooService>();

            return unityContainer;
        }

        [Fact]
        public void UnityUseCaseExample()
        {
            var unityContainer = GetInMemoryContainerExample();
            // Get a ModelFooService using the container, which will then resolve dependencies
            var modelFooService = unityContainer.Resolve<ModelFooService>();
            var modelFoo = modelFooService.InsertOrUpdate(new ModelFoo { Name = "Test" });

            Assert.NotEqual(default(int), modelFoo.Id);

            var findModelFoo = modelFooService.Find(modelFoo.Id);
            Assert.NotNull(findModelFoo);

            Assert.Throws<RulesException<ModelFoo>>(() =>
                {
                    try
                    {
                        modelFooService.ValidateAndInsertOrUpdate(new ModelFoo { Name = "Test too big" });
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(ModelFooService.INVALIDLENGTH, ex.Errors[1].Message);
                        throw;
                    }
                });
        }
    }
}