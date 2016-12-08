using System;
using System.Collections.Generic;

namespace Arragro.Common.CacheProvider
{
    public class CacheItemList<T> : CacheItem, ICacheItemList<T>
    {
        public IEnumerable<T> Items { get; set; }

        public CacheItemList(
            string key,
            IEnumerable<T> item,
            CacheSettings cacheSettings)
            : base(key, cacheSettings)
        {
            Items = item;
        }
    }
}