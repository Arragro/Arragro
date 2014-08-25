using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using Arragro.Common.Tests.ModelsAndHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Arragro.Common.Tests.BusinessRules.UseCases
{
    public class BusinessRulesUseCase
    {
        public readonly List<ModelFoo> ModelFoos;

        public class ModelFooService : BusinessRulesBase<IRepository<ModelFoo, int>, ModelFoo, int>
        {
            public const string DuplicateName = "There is already a Model Foo with that name in the repository";
            public const string RequiredName = "The Name field is required";
            public const string RangeLengthName = "The Name field must have between 3 and 6 characters";

            public ModelFooService(IRepository<ModelFoo, int> modelFooRepository) : base(modelFooRepository) {}

            /* 
             * This function would be implemented further down the chain as
             * BusinessRulesBase provides the structure, not the implementation
             * which would be custom per Model.
             * 
             * This would occur on a InsertOrUpdate at the service layer.
             */
            public void ValidateModel(ModelFoo modelFoo)
            {
                if (RepositoryBase.All()
                        .Where(x => x.Id != modelFoo.Id 
                                 && x.Name == modelFoo.Name).Any())
                    RulesException.ErrorFor(x => x.Name, DuplicateName);

                if (String.IsNullOrEmpty(modelFoo.Name))
                    RulesException.ErrorFor(x => x.Name, RequiredName);

                else if (modelFoo.Name.Length < 2 || modelFoo.Name.Length > 6)
                    RulesException.ErrorFor(c => c.Name, RangeLengthName);
                
                if (RulesException.Errors.Any()) throw RulesException;
            }
        }

        public BusinessRulesUseCase()
        {
            ModelFoos = ModelFoos.InitialiseAndLoadModelFoos();
        }

        [Fact]
        public void CheckBusinessRulesBaseWorks()
        {
            var modelFooRepository = MockHelper.GetMockRepository(ModelFoos);
            var modelFooService = new ModelFooService(modelFooRepository);

            Assert.Throws<RulesException<ModelFoo>>(
                () =>
                {
                    try
                    {
                        modelFooService.ValidateModel(new ModelFoo { Id = 3, Name = "Test 2" });
                    }
                    catch (RulesException<ModelFoo> ex)
                    {
                        Assert.Equal(1, ex.Errors.Count());
                        Assert.Equal(ModelFooService.DuplicateName, ex.Errors[0].Message);
                        throw;
                    }
                });
            modelFooService.RulesException.Errors.Clear();

            Assert.Throws<RulesException<ModelFoo>>(
                () =>
                {
                    try
                    {
                        modelFooService.ValidateModel(new ModelFoo { Id = 3, Name = null });
                    }
                    catch (RulesException<ModelFoo> ex)
                    {
                        Assert.Equal(1, ex.Errors.Count());
                        Assert.Equal(ModelFooService.RequiredName, ex.Errors[0].Message);
                        throw;
                    }
                });
            modelFooService.RulesException.Errors.Clear();

            Assert.Throws<RulesException<ModelFoo>>(
                () =>
                {
                    try
                    {
                        modelFooService.ValidateModel(new ModelFoo { Id = 3, Name = "1" });
                    }
                    catch (RulesException<ModelFoo> ex)
                    {
                        Assert.Equal(1, ex.Errors.Count());
                        Assert.Equal(ModelFooService.RangeLengthName, ex.Errors[0].Message);
                        throw;
                    }
                });
            modelFooService.RulesException.Errors.Clear();

            Assert.Throws<RulesException<ModelFoo>>(
                () =>
                {
                    try
                    {
                        modelFooService.ValidateModel(new ModelFoo { Id = 3, Name = "1234567" });
                    }
                    catch (RulesException<ModelFoo> ex)
                    {
                        Assert.Equal(1, ex.Errors.Count());
                        Assert.Equal(ModelFooService.RangeLengthName, ex.Errors[0].Message);
                        throw;
                    }
                });
            modelFooService.RulesException.Errors.Clear();
        }
    }
}
