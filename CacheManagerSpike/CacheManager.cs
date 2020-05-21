using System;
using Microsoft.Extensions.Caching.Memory;

namespace CacheManagerSpike
{
    public class CacheManager
    {
        private readonly MemoryCache cache;

        public CacheManager()
            : this(new MemoryCacheOptions())
        {
        }

        public CacheManager(MemoryCacheOptions options)
        {
            cache = new MemoryCache(options);
        }

        public T GetOrAdd<T>(object key, DateTime expireOn, Func<object, T> factory)
        {
            return cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpiration = expireOn;
                return factory(key);
            });
        }
    }
}
