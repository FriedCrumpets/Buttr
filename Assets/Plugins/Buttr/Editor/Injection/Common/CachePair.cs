using System;

namespace Buttr.Editor.Injection {
    [Serializable]
    internal sealed class CachePair {
        public string key;
        public CachedEntry entry;
    }
}