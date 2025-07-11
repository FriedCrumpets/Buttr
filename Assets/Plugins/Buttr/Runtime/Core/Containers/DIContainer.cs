using System;
using System.Collections.Generic;

namespace Buttr.Core {
    /// <summary>
    /// Created from a <see cref="DIBuilder{TID}"/>. A container to retrieve objects from.
    /// </summary>
    /// <remarks>
    /// The container should be cached and kept for use until disposal. It's advised to dispose of containers when finished.
    /// </remarks>
    /// <typeparam name="TID">The ID for which behaviours are to be identified</typeparam>
    internal class DIContainer<TID> : IDIContainer<TID> {
        private readonly Dictionary<TID, IObjectResolver> m_Registry;
        
        internal DIContainer(Dictionary<TID, IObjectResolver> registry) {
            m_Registry = registry;
        }
        
        public Type Type => typeof(TID);
        
        public T Get<T>(TID id) {
            if (typeof(IHidden).IsAssignableFrom(typeof(T))) 
                throw new ObjectResolverException("Attempting to retrieve a Hidden object from a DIContainer");

            if (m_Registry.TryGetValue(id, out var resolver)) {
                return (T)resolver.Resolve();
            }

            return default;
        }
        
        public bool TryGet<T>(TID id, out T value) {
            if (typeof(IHidden).IsAssignableFrom(typeof(T))) 
                throw new ObjectResolverException("Attempting to retrieve a Hidden object from a DIContainer");
            
            var found = m_Registry.TryGetValue(id, out var resolver);
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
        }
    }
    
    /// <summary>
    /// Created from a <see cref="DIBuilder"/>. A container to retrieve objects from.
    /// </summary>
    /// <remarks>
    /// The container should be cached and kept for use until disposal. It's advised to dispose of containers when finished.
    /// </remarks>
    internal sealed class DIContainer : IDIContainer {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;

        internal DIContainer(Dictionary<Type, IObjectResolver> registry) {
            m_Registry = registry;
        }

        public T Get<T>() {
            if (typeof(IHidden).IsAssignableFrom(typeof(T)))
                throw new ObjectResolverException("Attempting to retrieve a Hidden object from a DIContainer");
            
            if (m_Registry.TryGetValue(typeof(T), out var resolver))
                return (T)resolver.Resolve();

            return default;
        }
        
        public bool TryGet<T>(out T value) {
            if (typeof(IHidden).IsAssignableFrom(typeof(T))) 
                throw new ObjectResolverException("Attempting to retrieve a Hidden object from a DIContainer");

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
        }
    }
}