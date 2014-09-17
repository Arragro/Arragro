using Arragro.Common.Logging;
using System;
using System.Collections.Generic;

namespace Arragro.Common.CacheProvider
{
    /// <summary>
    /// Extensions to simplify the Cache Provider and its functionality
    /// </summary>
    public static class Cache
    {
        private static ILog _log;

        static Cache()
        {
            _log = LogManager.GetLogger(typeof(Cache));
        }

        private static ICacheProvider CacheProvider
        {
            get { return CacheProviderManager.CacheProvider; }
        }

        public static bool IsNull<T>(this T obj)
        {
            return EqualityComparer<T>.Default.Equals(obj, default(T));
        }

        private static ICacheItem<T> GetCachedData<T>(string key)
        {
            _log.DebugFormat("Cache.GetCacheData:{0}", key);
            return CacheProvider.Get<T>(key);
        }

        private static T SaveCachedDataAndReturn<T>(string key, TimeSpan? cacheDuration, Func<T> func, bool slidingExpiration = true)
        {
            _log.DebugFormat("Cache.SaveCachedDataAndReturn:{0}", key);
            _log.DebugFormat("Cache.SaveCachedDataAndReturn - Invoke:{0}", key);
            T newData = func.Invoke();
            _log.DebugFormat("Cache.SaveCachedDataAndReturn - Set:{0}", key);
            CacheProvider.Set(key, newData, cacheDuration, slidingExpiration);
            return newData;
        }

        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            
            _log.DebugFormat("Cache.Get:{0}", key);
            if (CacheProvider != null)
            {
                _log.DebugFormat("Cache.Get - GetCachedData:{0}", key);
                ICacheItem<T> data = GetCachedData<T>(key);
                _log.DebugFormat("Cache.Get - ReturnCachedData:{0}", key);
                if (data == null)
                    return default(T);
                return data.Item;
            }
            return default(T);
        }

        public static T Get<T>(
            string key, Func<T> func)
        {
            return Get(key, null, func);
        }

        public static T Get<T>(
            string key, TimeSpan? cacheDuration, Func<T> func)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            _log.DebugFormat("Cache.Get:{0}", key);

            _log.DebugFormat("Cache.Get - GetCachedData:{0}", key);
            ICacheItem<T> data = GetCachedData<T>(key);
            if (data == null)
            {
                _log.DebugFormat("Cache.Get - SaveAndReturnCachedData:{0}", key);
                return SaveCachedDataAndReturn(key, cacheDuration, func);
            }

            _log.DebugFormat("Cache.Get - ReturnCachedData:{0}", key);
            return data.Item;
        }

        public static bool RemoveFromCache(string key, bool pattern = false)
        {
            _log.DebugFormat("Cache.RemoveFromCache:{0}:{1}", key, pattern);
            return CacheProvider.RemoveFromCache(key, pattern);
        }
    }
}
