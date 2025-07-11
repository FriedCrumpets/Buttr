using System;
using System.Collections.Generic;
using System.Linq;

namespace Buttr.Editor.Injection {
    [Serializable]
    internal sealed class CacheWrapper {
        public List<CachePair> entries = new();
        public Dictionary<string, CachedEntry> ToDict() {
            return entries.ToDictionary(entry => entry.key, entry => entry.entry);
        }
    }
}