﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Common.CacheProvider
{
    public static class CacheProviderManager
    {
        private readonly static object _locker = new object();

        private static ICacheProvider _cacheProvider;
        public static ICacheProvider CacheProvider
        {
            get
            {
                if (_cacheProvider == null)
                {
                    lock (_locker)
                    {
                        if (_cacheProvider == null)
                        {
                            _cacheProvider = MemoryCacheProvider.GetInstance();
                        }
                    }
                }
                return _cacheProvider;
            }
            set
            {
                _cacheProvider = value;
            }
        }
    }
}
