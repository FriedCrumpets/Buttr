using System;

namespace Buttr.Editor.Injection {
    [Serializable]
    internal sealed class CachedEntry {
        public string path;
        public string hash;
    }
}