using System;
using System.Collections.Generic;

namespace Buttr.Core {
    internal sealed class ScopeContainer : IDIContainer {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;

        private IDisposable m_Registration;
        
        internal ScopeContainer(Dictionary<Type, IObjectResolver> registry) {
            m_Registry = registry;
        }
        
        internal IDisposable ScopeRegistration {
            set { m_Registration = value; }
        }

        public T Get<T>() {
            if (typeof(IHidden).IsAssignableFrom(typeof(T))) 
                throw new ObjectResolverException("Attempting to retrieve a Hidden object from a ScopeContainer");
            
            if (m_Registry.TryGetValue(typeof(T), out var resolver))
                return (T)resolver.Resolve();

            return default;
        }
        
        public bool TryGet<T>(out T value) {
            if (typeof(IHidden).IsAssignableFrom(typeof(T))) 
                throw new ObjectResolverException("Attempting to retrieve a Hidden object from a ScopeContainer");

            var found = m_Registry.TryGetValue(typeof(T), out var resolver);
            value = found ? (T)resolver.Resolve() : default;
            return found;
        }

        public void Dispose() {
            foreach (var resolver in m_Registry.Values) {
                if (resolver.IsResolved && resolver.Resolve() is IDisposable disposable) {
                    disposable.Dispose();
                }
            }

            m_Registry.Clear();
            m_Registration.Dispose();
        }
    }
}