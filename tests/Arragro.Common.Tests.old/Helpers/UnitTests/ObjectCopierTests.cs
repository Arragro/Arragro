using Arragro.Common.Helpers;
using Arragro.TestBase;
using System;
using Xunit;

namespace Arragro.Common.Tests.Helpers.UnitTests
{
    public class ObjectCopierTests
    {
        [Fact]
        public void object_is_copied_successfully()
        {
            var modelFoo = new ModelFoo { Id = 1, Name = "Test" };
            var cloneModelFoo = modelFoo.Clone();

            Assert.False(object.ReferenceEquals(modelFoo, cloneModelFoo));
        }
    }
}