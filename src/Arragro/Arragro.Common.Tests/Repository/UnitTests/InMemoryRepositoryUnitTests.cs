using Arragro.Common.Repository;
using Arragro.Common.Tests.ModelsAndHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.Common.Tests.Repository.UnitTests
{
    public class InMemoryRepositoryUnitTests
    {
        private class ModelBarGuid
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        private class ModelBarInt
        {
            public int ModelBarIntId { get; set; }
        }

        private class ModelBarIntKey
        {
            [Key]
            public int Test { get; set; }
        }

        private class ModelBarFailOnKey
        {
            public int Nothing { get; set; }
        }

        private readonly InMemoryRepository<ModelFoo, int> _modelFooRepository;
        private readonly InMemoryRepository<ModelBarGuid, Guid> _modelBarGuidRepository;
        private readonly InMemoryRepository<ModelBarInt, int> _modelBarIntRepository;
        private readonly InMemoryRepository<ModelBarIntKey, int> _modelBarIntKeyRepository;
        
        public InMemoryRepositoryUnitTests()
        {
            _modelFooRepository = new InMemoryRepository<ModelFoo, int>();
            _modelBarGuidRepository = new InMemoryRepository<ModelBarGuid, Guid>();
            _modelBarIntRepository = new InMemoryRepository<ModelBarInt, int>();
            _modelBarIntKeyRepository = new InMemoryRepository<ModelBarIntKey, int>();
        }

        [Fact]
        public void TestNoFind()
        {
            var noFind = _modelFooRepository.Find(0);
            Assert.Null(noFind);
        }

        [Fact]
        public void TestInsertFindAndDelete()
        {
            var test1 = _modelFooRepository.InsertOrUpdate(new ModelFoo { Name = "Test 1" }, true);
            Assert.Equal(1, test1.Id);
            Assert.Equal(1, _modelFooRepository.Find(test1.Id).Id);
            _modelFooRepository.Delete(test1.Id);
        }

        [Fact]
        public void TestNoFindModelBarGuid()
        {
            var noFind = _modelBarGuidRepository.Find(Guid.NewGuid());
            Assert.Null(noFind);
        }

        [Fact]
        public void TestInsertFindAndDeleteModelBarGuid()
        {
            var test1 = _modelBarGuidRepository.InsertOrUpdate(new ModelBarGuid { Name = "Test 1" }, true);
            Assert.NotEqual(default(Guid), test1.Id);
            Assert.Equal(test1.Id, _modelBarGuidRepository.Find(test1.Id).Id);
            _modelBarGuidRepository.Delete(test1.Id);
        }

        [Fact]
        public void TestInsertFindAndDeleteModelBarInt()
        {
            var test1 = _modelBarIntRepository.InsertOrUpdate(new ModelBarInt(), true);
            Assert.Equal(1, test1.ModelBarIntId);
            Assert.Equal(test1.ModelBarIntId, _modelBarIntRepository.Find(test1.ModelBarIntId).ModelBarIntId);
            _modelBarIntRepository.Delete(test1.ModelBarIntId);
        }

        [Fact]
        public void TestInsertFindAndDeleteModelBarIntKey()
        {
            var test1 = _modelBarIntKeyRepository.InsertOrUpdate(new ModelBarIntKey(), true);
            Assert.Equal(1, test1.Test);
            Assert.Equal(test1.Test, _modelBarIntKeyRepository.Find(test1.Test).Test);
            _modelBarIntKeyRepository.Delete(test1.Test);
        }

        [Fact]
        public void FailOnBadModel()
        {
            Assert.Throws(typeof(Exception), () => new InMemoryRepository<ModelBarFailOnKey, int>());
        }
    }
}
