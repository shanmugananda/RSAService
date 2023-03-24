using Microsoft.Extensions.Caching.Memory;

namespace RoadSideAssistance.Services
{
    public interface IRSAMemoryCache
    {
        void Remove(string key);
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan ts);
    }

    public class WrapperClsMemoryCache: IRSAMemoryCache
    {
        private IMemoryCache _cache;
        public WrapperClsMemoryCache(IMemoryCache cache)
        {
            _cache=cache;
        }
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void Set<T>(string key, T value, TimeSpan ts)
        {
            _cache.Set<T>(key, value, ts);
        }
    }
}
