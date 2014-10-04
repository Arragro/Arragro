using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using Arragro.Common.ServiceBase;
using Arragro.TestBase;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Arragro.Common.Tests.Services.UseCase
{
    public class ServiceUseCase
    {
        public readonly List<ModelFoo> ModelFoos;

        public ServiceUseCase()
        {
            ModelFoos = ModelFoos.InitialiseAndLoadModelFoos();
        }

        [Fact]
        public void ModelFooServiceInsertOrUpdateUseCase()
        {
            var modelFooRepository = MockHelper.GetMockRepository(ModelFoos);
            var modelFooService = new ModelFooService(modelFooRepository);

            Assert.Throws<RulesException<ModelFoo>>(
                () =>
                {
                    try
                    {
                        modelFooService.EnsureValidModel(new ModelFoo());
                    }
                    catch (RulesException<ModelFoo> ex)
                    {
                        Assert.Equal(1, ex.Errors.Count());
                        Assert.Equal(ModelFooService.RequiredName, ex.Errors[0].Message);
                        throw;
                    }
                });

            modelFooService = new ModelFooService(modelFooRepository);

            modelFooService.InsertOrUpdate(new ModelFoo { Name = "Test" });
            Assert.Equal(3, ModelFoos.Count);
            Assert.Equal("Test", ModelFoos[2].Name);
        }
    }
}