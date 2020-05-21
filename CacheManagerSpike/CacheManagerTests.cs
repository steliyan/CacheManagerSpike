using System;
using System.Threading.Tasks;
using Xunit;

namespace CacheManagerSpike
{
    public class CacheManagerTests
    {
        private readonly CacheManager cacheManager;
        private readonly Func<object, string> factory;
        private int called;

        public CacheManagerTests()
        {
            called = 0;
            factory = key =>
            {
                called++;
                return $"{key} {called}";
            };
            cacheManager = new CacheManager();
        }

        [Fact]
        public async Task ShouldUseCache_WhenSingleValueIsNotExpired()
        {
            var k11 = cacheManager.GetOrAdd("key1", DateTime.Now.AddSeconds(1), factory);
            await Task.Delay(100);
            var k12 = cacheManager.GetOrAdd("key1", DateTime.MaxValue, factory);

            Assert.Equal("key1 1", k11);
            Assert.Equal("key1 1", k12);
            Assert.Equal(1, called);
        }

        [Fact]
        public async Task ShouldNotUseCache_WhenSingleValueIsExpired()
        {
            var k11 = cacheManager.GetOrAdd("key1", DateTime.Now.AddSeconds(1), factory);
            await Task.Delay(1000);
            var k12 = cacheManager.GetOrAdd("key1", DateTime.MaxValue, factory);

            Assert.Equal("key1 1", k11);
            Assert.Equal("key1 2", k12);
            Assert.Equal(2, called);
        }

        [Fact]
        public void ShouldUseCache_WithTwoValues()
        {
            var k11 = cacheManager.GetOrAdd("key1", DateTime.Now.AddSeconds(1), factory);
            var k21 = cacheManager.GetOrAdd("key2", DateTime.Now.AddSeconds(1), factory);

            Assert.Equal("key1 1", k11);
            Assert.Equal("key2 2", k21);
            Assert.Equal(2, called);
        }

        [Fact]
        public async Task ShouldNotUseCache_WithTwoValuesAndOneIsExpired()
        {
            var k11 = cacheManager.GetOrAdd("key1", DateTime.Now.AddSeconds(1), factory);
            var k21 = cacheManager.GetOrAdd("key2", DateTime.Now.AddSeconds(1), factory);
            await Task.Delay(1000);
            var k12 = cacheManager.GetOrAdd("key1", DateTime.Now.AddSeconds(1), factory);

            Assert.Equal("key1 1", k11);
            Assert.Equal("key2 2", k21);
            Assert.Equal("key1 3", k12);
            Assert.Equal(3, called);
        }
    }
}
