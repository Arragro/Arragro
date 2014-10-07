using System;
using System.Collections.Generic;

namespace Arragro.Common.CacheProvider
{
    public interface ICacheItem
    {
        Guid Identifier { get; }
        string Key { get; }
        DateTime CreatedDate { get; }
        DateTime? Expiration { get; }
        TimeSpan? CacheDuration { get; }
        bool SlidingExpiration { get; }
        int ByteLength { get; }
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