using Arragro.Common.Tests.BusinessRules.UseCases;
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
                    var businessRulesBase = new ModelFooService(null);
                });
        }
    }
}
