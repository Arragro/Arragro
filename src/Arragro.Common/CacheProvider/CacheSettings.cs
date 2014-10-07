using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Common.CacheProvider
{
    public class CacheSettings
    {
        public TimeSpan? CacheDuration { get; private set; }

        public bool SlidingExpiration { get; private set; }

        public CacheSettings()
        {
            CacheDuration = null;
            SlidingExpiration = false;
        }

        public CacheSettings(
            TimeSpan? cacheDuration,
            bool slidingExpiration = false)
        {
            CacheDuration = cacheDuration;
            SlidingExpiration = slidingExpiration;
        }
    }
}