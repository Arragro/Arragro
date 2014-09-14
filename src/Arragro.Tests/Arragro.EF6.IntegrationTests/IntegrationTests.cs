using Arragro.Common.Repository;
using Arragro.TestBase;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Arragro.EF6.IntegrationTests
{
    public class IntegrationTests
    {
        private void WithDbContext(Action<FooContext> action)
        {
            using (var context = new FooContext())
            {
                action.Invoke(context);
            }
        }

        public IntegrationTests()
        {
            WithDbContext(x =>
                {
                    if (x.Database.Exists())
                        x.Database.Delete();
                    x.Database.CreateIfNotExists();
                });
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
        public void use_ModelFooService_to_interact_with_SqlServerCE()
        {
            using (var context = new FooContext())
            {
                var modelFooRepository = new ModelFooRepository(context);
                var modelFooService = new ModelFooService(modelFooRepository);

                var modelFoo = modelFooService.InsertOrUpdate(new ModelFoo { Name = "Test" });
                modelFooService.SaveChanges();

                Assert.NotSame(default(int), modelFoo);
            }
        }

        [Fact]
        public void use_ModelFooService_to_interact_with_InMemoryRepository()
        {
            var modelFooRepository = new InMemoryRepository<ModelFoo, int>();
            var modelFooService = new ModelFooService(modelFooRepository);

            var modelFoo = modelFooService.InsertOrUpdate(new ModelFoo { Name = "Test" });
            modelFooService.SaveChanges();

            Assert.NotSame(default(int), modelFoo);
        }
    }
}
