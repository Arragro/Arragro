using System;

namespace Arragro.Common.CacheProvider
{
    [Serializable]
    public class CacheItem : ICacheItem
    {
        protected DateTime? GetExpiration(TimeSpan? cacheDuration)
        {
            DateTime? expiration = DateTime.Now;

            if (cacheDuration.HasValue)
                expiration = expiration.Value.Add(cacheDuration.Value);
            else
                expiration = null;

            return expiration;
        }

        public Guid Identifier { get; protected set; }
        public string Key { get; protected set; }
        public DateTime CreatedDate { get; protected set; }
        public DateTime? Expiration { get; protected set; }
        public TimeSpan? CacheDuration { get; protected set; }
        public bool SlidingExpiration { get; protected set; }
        public int ByteLength { get; protected set; }

        public CacheItem(
            string key,
            CacheSettings cacheSettings)
        {
            Identifier = Guid.NewGuid();
            Key = key;
            CreatedDate = DateTime.Now;
            Expiration = GetExpiration(cacheSettings.CacheDuration);
            CacheDuration = cacheSettings.CacheDuration;
            SlidingExpiration = cacheSettings.SlidingExpiration;
        }
    }

    [Serializable]
    public class CacheItem<T> : CacheItem, ICacheItem<T>
    {
        public T Item { get; set; }

        public CacheItem(
            string key,
            T item,
            CacheSettings cacheSettings)
            : base(key, cacheSettings)
        {
            Item = item;
        }
    }
}