﻿using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using Arragro.TestBase;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Arragro.Common.Tests.BusinessRules.UseCases
{
    public class BusinessRulesUseCase
    {
        public readonly List<ModelFoo> ModelFoos;

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
                        Assert.Single(ex.Errors);
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
                        Assert.Single(ex.Errors);
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
                        Assert.Single(ex.Errors);
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
                        Assert.Equal(2, ex.Errors.Count());
                        Assert.Equal(ModelFooService.RangeLengthName, ex.Errors[1].Message);
                        Assert.True(ex.ContainsErrorForProperty("Name"));
                        throw;
                    }
                });
            modelFooService.RulesException.Errors.Clear();

            Assert.Throws<RulesException<ModelFoo>>(
                () =>
                {
                    try
                    {
                        modelFooService.ValidateModel(new ModelFoo { Id = 3 });
                    }
                    catch (RulesException<ModelFoo> ex)
                    {
                        Assert.Single(ex.Errors);
                        Assert.True(ex.ContainsErrorForProperty("Name"));
                        throw;
                    }
                });
            modelFooService.RulesException.Errors.Clear();
        }
    }
}