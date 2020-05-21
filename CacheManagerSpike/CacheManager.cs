using System;
using Microsoft.Extensions.Caching.Memory;

namespace CacheManagerSpike
{
    public class CacheManager<T>
    {
        private readonly MemoryCache cache;
        private readonly Func<object, DateTime, Func<ICacheEntry, T>> createAbsoluteExpirationFactory;

        public CacheManager(Func<object, T> factory)
            : this(factory, new MemoryCacheOptions())
        {
        }

        public CacheManager(Func<object, T> factory, MemoryCacheOptions options)
        {
            cache = new MemoryCache(options);

            createAbsoluteExpirationFactory = (key, expireOn) => e =>
            {
                e.AbsoluteExpiration = expireOn;
                return factory(key);
            };
        }

        public T GetOrAdd(object key, DateTime expireOn)
        {
            var factory = createAbsoluteExpirationFactory(key, expireOn);
            return cache.GetOrCreate(key, factory);
        }
    }
}
