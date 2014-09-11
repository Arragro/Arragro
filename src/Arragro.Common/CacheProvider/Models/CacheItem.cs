using System;

namespace Arragro.Common.CacheProvider
{
    [Serializable]
    public class CacheItem : ICacheItem
    {
        public Guid Identifier { get; set; }
        public string Key { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? Expiration { get; set; }
        public TimeSpan? CacheDuration { get; set; }
        public bool SlidingExpiration { get; set; }
        public int ByteLength { get; set; }
    }

    [Serializable]
    public class CacheItem<T> : CacheItem, ICacheItem<T>
    {
        public T Item { get; set; }
        public CacheItem() { }
        public CacheItem(
            string key, T item,
            DateTime? expiration, TimeSpan? cacheDuration,
            bool slidingExpiration)
        {
            Identifier = Guid.NewGuid();
            Key = key;
            Item = item;
            CreatedDate = DateTime.Now;
            Expiration = expiration;
            CacheDuration = cacheDuration;
            SlidingExpiration = slidingExpiration;
        }
    }
}