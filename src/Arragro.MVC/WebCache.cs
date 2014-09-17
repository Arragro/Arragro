using Arragro.Common.CacheProvider;
using Arragro.Common.Logging;
using System;
using System.Web;

namespace Arragro.MVC
{
    public static class WebCache
    {
        private static ILog _log;

        static WebCache()
        {
            _log = LogManager.GetLogger(typeof(WebCache));
        }

        private static object Locker = new object();
        private static bool _webCacheEnabled = true;
        public static bool WebCacheEnabled
        {
            get { return _webCacheEnabled; }
            private set
            {
                lock (Locker)
                {
                    _webCacheEnabled = value;
                }
            }
        }

        private static T ProcessRequest<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (NullReferenceException)
            {
                WebCacheEnabled = false;
                return default(T);
            }
        }

        private static T SaveHttpCacheAndReturn<T>(string key, T data)
        {
            if (!_webCacheEnabled)
                return data;

            return ProcessRequest(
                () =>
                {
                    HttpContext.Current.Items[key] = data;
                    return data;
                });
        }

        private static T GetHttpCache<T>(string key)
        {
            if (!_webCacheEnabled)
                return default(T);

            return ProcessRequest(
                () =>
                {
                    if (HttpContext.Current.Items[key] != null)
                    {
                        var httpData = (T)HttpContext.Current.Items[key];
                        return httpData;
                    }
                    return default(T);
                });
        }

        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var httpData = GetHttpCache<T>(key);
            if (!httpData.IsNull()) return httpData;

            return Cache.Get<T>(key);
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

            var httpData = GetHttpCache<T>(key);
            if (httpData != null) return httpData;

            var data = Cache.Get<T>(key, cacheDuration, func);
            SaveHttpCacheAndReturn(key, data);

            return data;
        }
    }
}
