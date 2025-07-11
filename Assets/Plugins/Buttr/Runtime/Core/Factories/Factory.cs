using System;

namespace Buttr.Core {
    internal static class Factory {
        public static Func<TConcrete> Empty<TConcrete>() => null;
    }
}