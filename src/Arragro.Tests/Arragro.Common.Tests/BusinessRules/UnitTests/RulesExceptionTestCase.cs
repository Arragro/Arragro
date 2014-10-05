using Arragro.Common.BusinessRules;
using Arragro.Common.Repository;
using Arragro.Common.ServiceBase;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Arragro.Common.Tests.BusinessRules.UnitTests
{
    public class RulesExceptionTestCase
    {
        public class ValidateTest
        {
            public int ValidateTestId { get; set; }

            [Range(0, 1)]
            public decimal DecimalProperty { get; set; }

            [Required]
            public string StringProperty { get; set; }
        }

        public class ValidateTestService : Service<IRepository<ValidateTest, int>, ValidateTest, int>
        {
            public ValidateTestService(IRepository<ValidateTest, int> repository)
                : base(repository)
            {
            }

            protected override void ValidateModelRules(ValidateTest model)
            {
            }

            public override ValidateTest InsertOrUpdate(ValidateTest model)
            {
                throw new System.NotImplementedException();
            }
        }

        [Fact]
        public void RulesException_validation_converts_successfully()
        {
            var validateTestRepository = new InMemoryRepository<ValidateTest, int>();
            var validateTestService = new ValidateTestService(validateTestRepository);
            var validateTest = new ValidateTest
            {
                StringProperty = String.Empty,
                DecimalProperty = 2M
            };

            Assert.Throws<RulesException<ValidateTest>>(
                () =>
                {
                    try
                    {
                        validateTestService.ValidateModel(validateTest);
                    }
                    catch (RulesException ex)
                    {
                        Assert.Equal(2, ex.Errors.Count);
                        throw ex;
                    }
                });
        }
    }
}