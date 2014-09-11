using System;
using System.Collections.Generic;

namespace Arragro.Common.CacheProvider
{
    [Serializable]
    public class CacheItemList<T> : CacheItem, ICacheItemList<T>
    {
        public IEnumerable<T> Items { get; set; }

        public CacheItemList() { }

        public CacheItemList(
            string key, IEnumerable<T> item,
            DateTime? expiration, TimeSpan? cacheDuration,
            bool slidingExpiration)
        {
            Identifier = Guid.NewGuid();
            Key = key;
            Items = item;
            CreatedDate = DateTime.Now;
            Expiration = expiration;
            CacheDuration = cacheDuration;
            if (cacheDuration.HasValue)
                SlidingExpiration = slidingExpiration;
        }
    }
}