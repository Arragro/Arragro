using Arragro.Common.CacheProvider;
using System.Threading;
using Xunit;

namespace Arragro.CacheProvider.Redis.Integration.Tests
{ 
    public class Foo
    {
        public string Bar { get; set; }
    }

    public class RedisCacheProviderTests
    {
        public RedisCacheProviderTests()
        {
            Common.CacheProvider.CacheProviderManager.CacheProvider = new RedisCacheProvider("127.0.0.1");
        }

        [Fact]
        public void set_cache_setting_stores()
        {
            Common.CacheProvider.CacheProviderManager.CacheProvider.Set("test", new Foo { Bar = "Test" }, new CacheSettings(new System.TimeSpan(0, 0, 0, 0, 100), false));
            var bar = Common.CacheProvider.CacheProviderManager.CacheProvider.Get<Foo>("test");
            Assert.Equal("Test", bar.Item.Bar);
            bar = Common.CacheProvider.CacheProviderManager.CacheProvider.Get<Foo>("test");
            Common.CacheProvider.CacheProviderManager.CacheProvider.RemoveFromCache("test", false);
        }

        [Fact]
        public void set_cache_setting_stores_and_expires()
        {
            Common.CacheProvider.CacheProviderManager.CacheProvider.Set("test", new Foo { Bar = "Test" }, new CacheSettings(new System.TimeSpan(0, 0, 0, 0, 100), false));
            Thread.Sleep(101);
            var bar = Common.CacheProvider.CacheProviderManager.CacheProvider.Get<Foo>("test");
            Assert.Null(bar);
        }
    }
}
