using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Buttr.Editor.Injection {
    internal static class InjectionCacheManager {
        public static Dictionary<string, CachedEntry> LoadCache(this InjectionConfiguration configuration) {
            return File.Exists(configuration.CachePath)
                ? JsonUtility.FromJson<CacheWrapper>(File.ReadAllText(configuration.CachePath)).ToDict()
                : new Dictionary<string, CachedEntry>();
        }

        public static void SaveCache(this InjectionConfiguration configuration, Dictionary<string, CachedEntry> cache) {
            var wrapper = new CacheWrapper {
                entries = cache.Select(kvp => new CachePair { key = kvp.Key, entry = kvp.Value }).ToList()
            };
            if (File.Exists(configuration.CachePath) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(configuration.CachePath));
            
            File.WriteAllText(configuration.CachePath, JsonUtility.ToJson(wrapper));
        }
    }
}