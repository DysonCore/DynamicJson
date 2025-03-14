using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DysonCore.DynamicJson.SafeStringEnumParser
{
    public static class SafeStringEnumCacheRegistry
    {
        private static readonly Dictionary<string, Dictionary<Type, object>> FragmentedCache = new ();
        private static readonly ConcurrentDictionary<Type, object> Cache = new ();
        private static bool _cacheChanged = true;

        public static IReadOnlyDictionary<Type, object> DefaultsMap => GetCache();

        public static void Register(string fragment, Dictionary<Type, object> cache)
        {
            FragmentedCache[fragment] = cache;
            _cacheChanged = true;
            
        }
        
        private static ConcurrentDictionary<Type, object> GetCache()
        {
            if (!_cacheChanged)
            {
                return Cache;
            }

            
            foreach (KeyValuePair<string,Dictionary<Type,object>> cacheFragment in FragmentedCache)
            {
                Dictionary<Type, object> fragment = cacheFragment.Value;
                foreach (KeyValuePair<Type,object> pair in fragment)
                {
                    Cache[pair.Key] = pair.Value;
                }
            }

            return Cache;
        }
    }
}