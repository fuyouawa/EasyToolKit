using System;
using System.Collections.Generic;

namespace EasyToolKit.Core.Editor
{
    public interface IGUIContextCache<TKey, TValue>
    {
        bool TryGet(TKey key, out TValue value);
        TValue GetOrCreate(TKey key, Func<TValue> factory);
        TValue GetOrCreate(TKey key);
        void Set(TKey key, TValue value);
        void ClearExpired();
        void Clear();
    }

    public class GUIContextCache<TKey, TValue> : IGUIContextCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, DateTime> _accessTimestamps = new Dictionary<TKey, DateTime>();
        private readonly float _timeoutSeconds;

        public GUIContextCache(float timeoutSeconds = 5f)
        {
            _timeoutSeconds = timeoutSeconds;
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (_cache.TryGetValue(key, out value))
            {
                _accessTimestamps[key] = DateTime.Now;
                return true;
            }
            value = default;
            return false;
        }

        public TValue GetOrCreate(TKey key, Func<TValue> factory)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                _accessTimestamps[key] = DateTime.Now;
                return value;
            }

            var newValue = factory();
            Set(key, newValue);
            return newValue;
        }

        public TValue GetOrCreate(TKey key)
        {
            return GetOrCreate(key, () => typeof(TValue).CreateInstance<TValue>());
        }

        public void Set(TKey key, TValue value)
        {
            _cache[key] = value;
            _accessTimestamps[key] = DateTime.Now;
        }

        public bool ContainsKey(TKey key)
        {
            if (_cache.ContainsKey(key))
            {
                _accessTimestamps[key] = DateTime.Now;
                return true;
            }
            return false;
        }

        public void ClearExpired()
        {
            var currentTime = DateTime.Now;
            var keysToRemove = new List<TKey>();

            foreach (var kvp in _accessTimestamps)
            {
                if ((currentTime - kvp.Value).TotalSeconds > _timeoutSeconds)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _accessTimestamps.Remove(key);
            }
        }

        public void Clear()
        {
            _cache.Clear();
            _accessTimestamps.Clear();
        }
    }
}
