using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using Arragro.Common.Tests.ModelsAndHelpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.Common.Tests.BusinessRules.UnitTests
{
    /*
     * Not replicating the functionality demonstrated in BusinessRulesUseCase as the
     * AuditableBusinessRulesBase inherits BusinessRulesBase
     */
    public class AuditableBusinessRulesUnitTests
    {
        public class ModelFooIntService : AuditableBusinessRulesBase<IRepository<ModelFooInt, int>, ModelFooInt, int, int>
        {
            public ModelFooIntService(IRepository<ModelFooInt, int> modelFooRepository) : base(modelFooRepository) { }

            public ModelFooInt TestAddOrUpdateAudit(ModelFooInt modelFooInt, int userId, bool add)
            {
                AddOrUpdateAudit(modelFooInt, userId, add);
                return modelFooInt;
            }
        }

        public class ModelFooGuidService : AuditableBusinessRulesBase<IRepository<ModelFooGuid, int>, ModelFooGuid, int, Guid>
        {
            public ModelFooGuidService(IRepository<ModelFooGuid, int> modelFooRepository) : base(modelFooRepository) { }

            public ModelFooGuid TestAddOrUpdateAudit(ModelFooGuid modelFooGuid, Guid userId, bool add)
            {
                AddOrUpdateAudit(modelFooGuid, userId, add);
                return modelFooGuid;
            }
        }

        [Fact]
        public void TestAuditableFunctionalityAgainstIntModel()
        {
            var startDateTime = DateTime.Now.AddSeconds(-1);

            var mockRepository = new Mock<IRepository<ModelFooInt, int>>();
            var modelFooIntService = new ModelFooIntService(mockRepository.Object);

            var model = modelFooIntService.TestAddOrUpdateAudit(new ModelFooInt { Id = 1 }, 1, true);
            Assert.Equal(1, model.CreatedBy);
            Assert.True(model.CreatedDate > startDateTime);
            Assert.Equal(1, model.ModifiedBy);
            Assert.True(model.ModifiedDate == model.CreatedDate);

            model = modelFooIntService.TestAddOrUpdateAudit(model, 2, false);
            Assert.Equal(1, model.CreatedBy);
            Assert.True(model.CreatedDate > startDateTime);
            Assert.Equal(2, model.ModifiedBy);
            Assert.True(model.ModifiedDate > model.CreatedDate);
        }

        [Fact]
        public void TestAuditableFunctionalityAgainstGuidModel()
        {
            var startDateTime = DateTime.Now.AddSeconds(-1);

            var mockRepository = new Mock<IRepository<ModelFooGuid, int>>();
            var modelFooGuidService = new ModelFooGuidService(mockRepository.Object);

            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            var model = modelFooGuidService.TestAddOrUpdateAudit(new ModelFooGuid { Id = 1 }, user1, true);
            Assert.Equal(user1, model.CreatedBy);
            Assert.True(model.CreatedDate > startDateTime);
            Assert.Equal(user1, model.ModifiedBy);
            Assert.True(model.ModifiedDate == model.CreatedDate);

            model = modelFooGuidService.TestAddOrUpdateAudit(model, user2, false);
            Assert.Equal(user1, model.CreatedBy);
            Assert.True(model.CreatedDate > startDateTime);
            Assert.Equal(user2, model.ModifiedBy);
            Assert.True(model.ModifiedDate > model.CreatedDate);
        }
    }
}
