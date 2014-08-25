using Arragro.Common.Repository;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace Arragro.Common.Tests.Repository.UseCase
{
    public class IRepositoryUseCase
    {
        public class ModelFoo
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public readonly List<ModelFoo> _modelFoos;

        public IRepositoryUseCase()
        {
            _modelFoos = new List<ModelFoo>();
            _modelFoos.Add(new ModelFoo { Id = 1, Name = "Test 1" });
            _modelFoos.Add(new ModelFoo { Id = 2, Name = "Test 2" });
        }

        /// <summary>
        /// Builds a Mock repository as an example of how the pattern will work
        /// in the service layer.
        /// </summary>
        /// <returns>IRepository<ModelFoo, int></returns>
        private IRepository<ModelFoo, int> GetMockRepository()
        {
            //Instantiate the Mock
            var moqRepository = new Mock<IRepository<ModelFoo, int>>();
            
            //Build the methods exposed by the interface (not all will be required, but I have done anyway.
            //Later, this pattern can be put into a Base class using generics to make it work with any model.
            moqRepository.Setup(x => x.TurnOnOffLazyLoading(true)).Verifiable();
            moqRepository.Setup(x => x.Find(It.IsAny<int>())).Returns((int id) => _modelFoos.SingleOrDefault(x => x.Id == id));
            moqRepository.Setup(x => x.Delete(It.IsAny<int>())).Callback((int id) =>
                {
                    var modelFoo = _modelFoos.SingleOrDefault(f => f.Id == id);
                    if (modelFoo != null)
                        _modelFoos.Remove(modelFoo);
                });
            moqRepository.Setup(x => x.All()).Returns(_modelFoos.AsQueryable());
            moqRepository.Setup(x => x.InsertOrUpdate(It.IsAny<ModelFoo>(), It.IsAny<bool>()))
                .Returns((ModelFoo modelFoo, bool add) =>
                {
                    if (add)
                    {
                        modelFoo.Id = _modelFoos.Max(f => f.Id) + 1;
                        _modelFoos.Add(modelFoo);
                    }
                    else
                    {
                        _modelFoos[_modelFoos.FindIndex(f => f.Id == modelFoo.Id)] = modelFoo;
                    }
                    return modelFoo;
                });
            moqRepository.Setup(x => x.SaveChanges()).Returns(0);

            return moqRepository.Object;
        }


        [Fact]
        public void RepositoryUseCaseMoq()
        {
            var modelFooRepository = GetMockRepository();
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
