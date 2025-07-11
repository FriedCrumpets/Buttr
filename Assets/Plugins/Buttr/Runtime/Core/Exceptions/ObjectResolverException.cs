using System;

namespace Buttr.Core {
    /// <summary>
    /// Thrown if anything goes wrong during the resolving of an object. 
    /// </summary>
    public sealed class ObjectResolverException : Exception {
        public ObjectResolverException(string message) : base(message) { }
    }
}