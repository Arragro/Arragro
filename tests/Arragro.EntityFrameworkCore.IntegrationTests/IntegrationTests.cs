using Arragro.Common.Repository;
using Arragro.TestBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Arragro.EntityFrameworkCore.IntegrationTests
{
    public class IntegrationTests
    {
        private readonly DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<FooContext>().UseInMemoryDatabase();

        private void WithDbContext(Action<FooContext> action)
        {
            using (var context = new FooContext(optionsBuilder.Options))
            {
                action.Invoke(context);
            }
        }

        [Fact]
        public void add_record_to_database()
        {
            WithDbContext(x =>
                {
                    x.ModelFoos.Add(new ModelFoo { Name = "Test" });
                    x.SaveChanges();

                    var modelFoo = x.ModelFoos.Single();
                    Assert.Equal("Test", modelFoo.Name);
                    Assert.NotSame(default(int), modelFoo.Id);
                });
        }

        [Fact]
        public void use_ModelFooService_to_interact()
        {
            using (var context = new FooContext(optionsBuilder.Options))
            {
                var modelFooRepository = new ModelFooRepository(context);
                var modelFooService = new ModelFooService(modelFooRepository);

                var modelFoo = modelFooService.InsertOrUpdate(new ModelFoo { Name = "Test" });
                modelFooService.SaveChanges();

                Assert.NotSame(default(int), modelFoo);
            }
        }
    }
}
