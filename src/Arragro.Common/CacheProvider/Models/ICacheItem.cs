using System;
using System.Collections.Generic;

namespace Arragro.Common.CacheProvider
{
    public interface ICacheItem
    {
        Guid Identifier { get; set; }
        string Key { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? Expiration { get; set; }
        TimeSpan? CacheDuration { get; set; }
        bool SlidingExpiration { get; set; }
        int ByteLength { get; set; }
    }

    public interface ICacheItem<T> : ICacheItem
    {
        T Item { get; set; }
    }

    public interface ICacheItemList<T> : ICacheItem
    {
        IEnumerable<T> Items { get; set; }
    }
}