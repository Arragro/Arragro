using Arragro.Common.Tests.ModelsAndHelpers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Arragro.Common.Tests.Repository.UseCase
{
    public class IRepositoryUseCase
    {
        public readonly List<ModelFoo> _modelFoos;

        public IRepositoryUseCase()
        {
            _modelFoos = new List<ModelFoo>();
            _modelFoos.Add(new ModelFoo { Id = 1, Name = "Test 1" });
            _modelFoos.Add(new ModelFoo { Id = 2, Name = "Test 2" });
        }
        
        [Fact]
        public void RepositoryUseCaseMoq()
        {
            var modelFooRepository = MockHelper.GetMockRepository(_modelFoos);
            Assert.Equal(modelFooRepository.Find(1).Name, "Test 1");
            Assert.Equal(modelFooRepository.Find(2).Name, "Test 2");
            modelFooRepository.InsertOrUpdate(new ModelFoo { Name = "Test 3" }, true);
            Assert.Equal(modelFooRepository.Find(3).Name, "Test 3");
            modelFooRepository.InsertOrUpdate(new ModelFoo { Id = 3, Name = "Testy" }, false);
            Assert.Equal(modelFooRepository.All().Count(), 3);
            modelFooRepository.Delete(3);
            Assert.Equal(modelFooRepository.All().Count(), 2);
        }
    }
}
