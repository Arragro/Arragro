using Arragro.ObjectLogging;
using Newtonsoft.Json;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodstuffs.Core.Tests.ObjectLogging
{
    public class EnginTest
    {
        private class TestObject
        {
            public string Test1 { get; set; }
            public string Test2;

            private string Test3;

            public TestObject(string test3)
            {
                Test3 = test3;
            }
        }

        [Fact]
        public void test_engine_compare_returns_nothing()
        {
            var t1 = new TestObject("test1");
            var t2 = new TestObject("test2");

            Assert.Equal(Enumerable.Empty<ComparisonResult>(), CompareEngine.Compare(t1, t2));

            t1.Test1 = "test";
            t1.Test2 = "test";
            t2.Test1 = "test";
            t2.Test2 = "test";

            Assert.Equal(Enumerable.Empty<ComparisonResult>(), CompareEngine.Compare(t1, t2));
        }

        [Fact]
        public void test_engine_compare_returns_2_records()
        {
            var t1 = new TestObject("test1") { Test1 = "X", Test2 = "Y" };
            var t2 = new TestObject("test2") { Test1 = "Y", Test2 = "X" };

            var result = CompareEngine.Compare(t1, t2);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void test_engine_compare_returns_2_records_json_deserializes()
        {
            var t1 = new TestObject("test1") { Test1 = "X", Test2 = "Y" };
            var t2 = new TestObject("test2") { Test1 = "Y", Test2 = "X" };

            var result = CompareEngine.Compare(t1, t2);

            var json = JsonConvert.SerializeObject(result);
            var deserializedResults = JsonConvert.DeserializeObject<IEnumerable<ComparisonResult>>(json);

            Assert.Equal(2, deserializedResults.Count());
        }
    }
}
