using System;

namespace Buttr.Core {
    public sealed class ConfigurableException : Exception {
        public ConfigurableException(string message ) : base(message) { }
    }
}