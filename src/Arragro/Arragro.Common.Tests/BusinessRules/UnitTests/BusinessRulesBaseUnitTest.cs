using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using Arragro.Common.Tests.ModelsAndHelpers;
using System;
using Xunit;

namespace Arragro.Common.Tests.BusinessRules.UnitTests
{
    public class BusinessRulesBaseUnitTest
    {
        [Fact]
        public void TestBusinessRulesBaseFailsWhenUsingNullRepository()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    var businessRulesBase = new BusinessRulesBase<IRepository<ModelFoo, int>, ModelFoo, int>(null);
                });
        }
    }
}
