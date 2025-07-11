using System;

namespace Buttr.Core {
    internal static class ConfigurationFactory {
        public static Func<TConcrete, TConcrete> Empty<TConcrete>() {
            return t => t;
        } 
    }
}