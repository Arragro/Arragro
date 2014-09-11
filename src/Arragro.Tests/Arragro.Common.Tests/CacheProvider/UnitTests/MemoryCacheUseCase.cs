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

            cacheProvider.Set("Test", "Hello", new TimeSpan(0, 0, 0, 0, 10), true);

            var data = cacheProvider.Get<string>("Test");
            Assert.Equal("Hello", data.Item);

            Thread.Sleep(11);

            data = cacheProvider.Get<string>("Test");
            Assert.Null(data);
        }

        [Fact]
        public void TestCacheUseCaseWithMemoryCache()
        {
            Cache.Set("Hello", new TimeSpan(0, 0, 0, 0, 10), "Hello");
            var data = Cache.Get<string>("Hello");
            Assert.Equal("Hello", data);

            Thread.Sleep(11);
            Assert.Null(Cache.Get<string>("Hello"));
        }
    }
}
