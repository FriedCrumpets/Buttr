using System;

namespace Buttr.Injection {
    public sealed class InjectionException : Exception {
        public InjectionException(string message) : base(message) { }
    }
}