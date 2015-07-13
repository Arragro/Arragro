using System;
using System.Collections.Generic;
using System.Linq;
using Arragro.Common.CacheProvider;
using StackExchange.Redis;
using Newtonsoft.Json;
using Ionic.Zlib;
using Arragro.Redis.JsonHelpers;

namespace Arragro.Redis.CacheProvider
{
    public class RedisCacheProvider : ICacheProvider
    {
        private static string _connectionString = string.Empty;
        private static int _port = 6379;

        private static IncludePrivateStateContractResolver _contractResolver;
        private static JsonSerializerSettings _jsonSerializerSettings;

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
            return ConnectionMultiplexer.Connect(_connectionString);
        });

        static RedisCacheProvider()
        {
            _contractResolver = new IncludePrivateStateContractResolver();
            _jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = _contractResolver };
        }

        private byte[] ConvertStringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private byte[] CompressString(string str)
        {
            return ZlibStream.CompressString(str);
        }

        private string DecompressString(byte[] bytes)
        {
            return ZlibStream.UncompressString(bytes);
        }

        private CacheItem<T> DecompressAndDeserializeCacheItem<T>(byte[] bytes)
        {
            if (bytes == null)
                return null;
            return JsonConvert.DeserializeObject<CacheItem<T>>(ZlibStream.UncompressString(bytes), _jsonSerializerSettings);
        }

        private CacheItemList<T> DecompressAndDeserializeCacheItemList<T>(byte[] bytes)
        {
            if (bytes == null)
                return null;
            return JsonConvert.DeserializeObject<CacheItemList<T>>(ZlibStream.UncompressString(bytes), _jsonSerializerSettings);
        }

        private byte[] SerializeAndCompress<T>(CacheItem<T> cacheItem)
        {
            var json = JsonConvert.SerializeObject(cacheItem, _jsonSerializerSettings);
            return CompressString(json);
        }

        private byte[] SerializeAndCompress<T>(CacheItemList<T> cacheItem)
        {
            var json = JsonConvert.SerializeObject(cacheItem, _jsonSerializerSettings);
            return CompressString(json);
        }

        private static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public RedisCacheProvider(string host, int port = 6379)
        {
            _connectionString = host;
            _port = port;
            Connection.GetCounters();
        }


        public ICacheItem<T> Get<T>(string key)
        {
            var db = Connection.GetDatabase();
            var cacheItem = DecompressAndDeserializeCacheItem<T>(db.StringGet(key));

            if (cacheItem != null)
            {
                if (cacheItem.CacheSettings.SlidingExpiration)
                {
                    cacheItem.ResetExpiration();
                    if (cacheItem.CacheSettings.CacheDuration != null)
                        db.KeyExpire(key, cacheItem.CacheSettings.CacheDuration);
                }
            }
            return cacheItem;
        }

        public MasterKeys GetAllCacheItems()
        {
            var db = Connection.GetDatabase();
            var server = Connection.GetServer(_connectionString, _port);
            var data = new List<MasterKey>();

            var keys = server.Keys(db.Database, "*");
            data.AddRange(keys.Select(key => new MasterKey { Key = key, ByteLength = 0 }));

            return new MasterKeys { Keys = data };
        }

        public ICacheItemList<T> GetList<T>(string key)
        {
            var db = Connection.GetDatabase();
            var cacheItem = DecompressAndDeserializeCacheItemList<T>(db.StringGet(key));

            if (cacheItem != null)
            {
                if (cacheItem.CacheSettings.SlidingExpiration)
                {
                    cacheItem.ResetExpiration();
                    if (cacheItem.CacheSettings.CacheDuration != null)
                        db.KeyExpire(key, cacheItem.CacheSettings.CacheDuration);
                }
            }
            return cacheItem;
        }

        public void RemoveAll()
        {
            var db = Connection.GetDatabase();
            var server = Connection.GetServer(_connectionString, _port);
            server.FlushDatabase(db.Database);
        }

        public bool RemoveFromCache(string key, bool pattern)
        {
            var db = Connection.GetDatabase();
            var server = Connection.GetServer(_connectionString, _port);
            try
            {
                if (pattern)
                {
                    throw new NotImplementedException();
                }
                return db.KeyDelete(key);
            }
            catch
            {
                return false;
            }
        }

        public ICacheItem<T> Set<T>(string key, T data, CacheSettings cacheSettings)
        {
            var db = Connection.GetDatabase();

            var cacheItem = new CacheItem<T>(key, data, cacheSettings);
            db.StringSet(key, SerializeAndCompress(cacheItem));
            if (cacheItem.CacheSettings.CacheDuration.HasValue)
                db.KeyExpire(key, cacheItem.CacheSettings.CacheDuration);
            return cacheItem;
        }

        public ICacheItemList<T> SetList<T>(string key, IEnumerable<T> data, CacheSettings cacheSettings)
        {
            var db = Connection.GetDatabase();

            var cacheItemList = new CacheItemList<T>(key, data, cacheSettings);
            db.StringSet(key, SerializeAndCompress(cacheItemList));
            if (cacheItemList.CacheSettings.CacheDuration.HasValue)
                db.KeyExpire(key, cacheItemList.CacheSettings.CacheDuration);
            return cacheItemList;
        }
    }
}