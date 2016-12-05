using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace jTech.Common.Caching
{
    public interface IExpiringCache<T> where T : class
    {
        void AddOrUpdate(string key, IExpiringCachedItem<T> value);
        bool TryGetValue(string key, out T value);
        void PurgeCache();
    }

    public class ExpiringCache<TKey, TValue> where TValue : class
    {
        private readonly ConcurrentDictionary<TKey, IExpiringCachedItem<TValue>> _cache
            = new ConcurrentDictionary<TKey, IExpiringCachedItem<TValue>>();
        private readonly Func<TValue, DateTime, bool> _isExpired;

        public ExpiringCache(Func<TValue, DateTime, bool> isExpired)
        {
            _isExpired = isExpired;
        }

        public void AddOrUpdate(TKey key, IExpiringCachedItem<TValue> value)
        {
            _cache.AddOrUpdate(key, value, (k, v) => value);
            PurgeCache();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = null;
            IExpiringCachedItem<TValue> cached;
            if (_cache.TryGetValue(key, out cached))
            {
                if (cached != null)
                {
                    value = cached.Item;
                }
                return true;
            }
            return false;
        }

        public void PurgeCache()
        {
            var expiredItems = _cache.Where(kvp => kvp.Value.IsExpired)
                                     .ToList();

            foreach (KeyValuePair<TKey, IExpiringCachedItem<TValue>> kvp in expiredItems)
            {
                IExpiringCachedItem<TValue> item;
                _cache.TryRemove(kvp.Key, out item);

                if (item != null)
                {
                    item.Item = null;
                    item = null;
                }
            }
        }

    }
}
