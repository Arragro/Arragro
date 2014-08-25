using Arragro.Common.BusinessRules;
using System;
using Xunit;

namespace Arragro.Common.Tests.BusinessRules.UseCases
{
    public class AuditableUseCase
    {
        private class AuditableTestInt : IAuditable<int>
        {
            public int CreatedBy { get; set; }
            public int ModifiedBy { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }
        }

        private class ModelFoo : AuditableTestInt
        {
            public int EntityFooId { get; set; }
        }

        private class ModelBar : Auditable<Guid>
        {
            public int EntityBarId { get; set; }
        }

        [Fact]
        public void AuditableUseCaseTestFoo()
        {
            var entityFoo = new ModelFoo
            {
                EntityFooId = 1,
                CreatedBy = 1,
                ModifiedBy = 1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }

        [Fact]
        public void AuditableUseCaseTestBar()
        {
            var userId = Guid.NewGuid();
            var entityBar = new ModelBar
            {
                EntityBarId = 1,
                CreatedBy = userId,
                ModifiedBy = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
