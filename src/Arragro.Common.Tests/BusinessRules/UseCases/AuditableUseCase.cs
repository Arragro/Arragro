using Arragro.Common.BusinessRules;
using Arragro.Common.Tests.ModelsAndHelpers;
using System;
using Xunit;

namespace Arragro.Common.Tests.BusinessRules.UseCases
{
    public class AuditableUseCase
    {
        [Fact]
        public void AuditableUseCaseTestFoo()
        {
            var entityFoo = new ModelFooInt
            {
                Id = 1,
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
            var entityBar = new ModelFooGuid
            {
                Id = 1,
                CreatedBy = userId,
                ModifiedBy = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
