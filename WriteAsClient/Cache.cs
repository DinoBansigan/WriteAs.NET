using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace WriteAs.NET
{
    internal class Cache : IDisposable
    {
        private IMemoryCache _cache;
        private int _cacheExpirationInSeconds;
        private int _cacheSize;
        private Queue<string> _cacheKeysQueue;

        public Cache(int cacheExpirationInSeconds = 300, int cacheSize = 4)
        {
            _cacheExpirationInSeconds = cacheExpirationInSeconds;
            _cacheSize = cacheSize;
            _cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = _cacheSize });
            _cacheKeysQueue = new Queue<string>(_cacheSize);
        }

        public void Dispose()
        {
            if (_cache != null)
            {
                _cache.Dispose();
            }
        }

        internal T GetDataFromCache<T>(string cacheKey)
        {
            _cache.TryGetValue(cacheKey, out T post);

            return post;
        }

        internal void SaveDataToCache<T>(string cacheKey, T data)
        {
            MemoryCacheEntryOptions cacheEntryOptions = GetCacheEntryOptions();

            RemoveOldestCacheEntryIfCacheIsFull();

            _cacheKeysQueue.Enqueue(cacheKey);
            _cache.Set(cacheKey, data, cacheEntryOptions);
        }

        private MemoryCacheEntryOptions GetCacheEntryOptions()
        {
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(_cacheExpirationInSeconds))
                // Absolute expiration stops an item from being cached indefinitely.
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60))
                .SetSize(1);
            return cacheEntryOptions;
        }

        private void RemoveOldestCacheEntryIfCacheIsFull()
        {
            if (_cacheKeysQueue.Count == _cacheSize)
            {
                string cacheKeyToRemove = _cacheKeysQueue.Dequeue();
                _cache.Remove(cacheKeyToRemove);
            }
        }
    }
}
