using System;
using System.Collections.Generic;

namespace Arragro.Common.CacheProvider
{
    public interface ICacheProvider
    {
        ICacheItemList<T> GetList<T>(string key);

        ICacheItem<T> Get<T>(string key);

        ICacheItemList<T> SetList<T>(string key, IEnumerable<T> data, CacheSettings cacheSettings);

        ICacheItem<T> Set<T>(string key, T data, CacheSettings cacheSettings);

        MasterKeys GetAllCacheItems();

        void RemoveAll();

        bool RemoveFromCache(string key, bool pattern);
    }
}