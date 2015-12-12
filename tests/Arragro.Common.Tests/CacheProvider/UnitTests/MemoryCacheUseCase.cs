using Arragro.Common.CacheProvider;
using System;
using System.Threading;
using Xunit;

namespace Arragro.Common.Tests.CacheProvider.UnitTests
{
    public class MemoryCacheUseCase
    {
        [Fact]
        public void TestCacheProviderDefaultsToMemoryCacheProvider()
        {
            Assert.Equal(CacheProviderManager.CacheProvider.GetType(), typeof(MemoryCacheProvider));
        }

        [Fact]
        public void TestMemoryCacheProvider()
        {
            CacheProviderManager.CacheProvider = MemoryCacheProvider.GetInstance();
            var cacheProvider = CacheProviderManager.CacheProvider;
            var cacheSettingsShort = new CacheSettings(new TimeSpan(0, 0, 0, 0, 5), false);
            var cacheSettingsLong = new CacheSettings(new TimeSpan(0, 0, 0, 0, 10), true);

            cacheProvider.Set("Test", "Hello", cacheSettingsLong);
            cacheProvider.Set("TestShort", "Hello", cacheSettingsShort);

            var data = cacheProvider.Get<string>("Test");
            Assert.Equal("Hello", data.Item);

            Thread.Sleep(6);

            data = cacheProvider.Get<string>("Test");
            Assert.Equal("Hello", data.Item);

            data = cacheProvider.Get<string>("TestShort");
            Assert.Null(data);

            Thread.Sleep(11);

            data = cacheProvider.Get<string>("Test");
            Assert.Null(data);
        }

        [Fact]
        public void TestCacheUseCaseWithMemoryCache()
        {
            var cacheSettings = new CacheSettings(new TimeSpan(0, 0, 0, 0, 10), true);
            var data = Cache.Get<string>("Hello", () => "Hello", cacheSettings);
            Assert.Equal("Hello", data);

            Thread.Sleep(11);
            Assert.Null(Cache.Get<string>("Hello"));
        }
    }
}