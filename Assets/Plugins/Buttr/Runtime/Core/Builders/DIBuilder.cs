using System;
using System.Collections.Generic;

namespace Buttr.Core {
    /// <summary>
    /// Used to construct a container that retrieves objects by ID not type
    /// </summary>
    /// <remarks>
    /// There are no type similarity restrictions here. Each object resolved can be of a completely different type if you so choose
    ///
    /// Each object resolved here will not resolve to the static registry, nor will they be statically accessible.
    /// Resolvers listed here will resolve using the static registry however.
    /// </remarks>
    /// <typeparam name="TID">The ID used to identify the resolve in question</typeparam>
    public class DIBuilder<TID> : IDIBuilder<TID> {
        private readonly Dictionary<TID, IObjectResolver> m_Registry = new();
        private readonly List<IResolver> m_Resolvers = new();
        public Type Type => typeof(TID);
        
        public IConfigurable<TConcrete> AddTransient<TConcrete>(TID id) {
            if (typeof(TConcrete).IsInterface)
                throw new ArgumentException($"{typeof(TConcrete)} cannot be an interface", nameof(TConcrete));
            
            var resolver =  new IDTransientObjectResolver<TID, TConcrete>(id, m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddSingleton<TConcrete>(TID id) {
            if (typeof(TConcrete).IsInterface)
                throw new ArgumentException($"{typeof(TConcrete)} cannot be an interface", nameof(TConcrete));
            
            var resolver =  new IDSingletonObjectResolver<TID, TConcrete>(id, m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IDIContainer<TID> Build() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();

            return new DIContainer<TID>(m_Registry);
        }
    }
    
    /// <summary>
    /// Used to construct a container for resolving objects
    /// </summary>
    /// <remarks>
    /// Each object resolved here will not resolve to the static registry, nor will they be statically accessible.
    /// Resolvers listed here will first resolve using the builders registry, if the required object is not found it will use the static registry.
    /// </remarks>
    public class DIBuilder : IDIBuilder {
        private readonly Dictionary<Type, IObjectResolver> m_Registry = new();
        private readonly List<IResolver> m_Resolvers = new();

        public IConfigurable<TConcrete> AddTransient<TConcrete>() {
            if (typeof(TConcrete).IsInterface)
                throw new ArgumentException($"{typeof(TConcrete)} cannot be an interface when added this way", nameof(TConcrete));
            
            var resolver = new TransientObjectResolver<TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>()  where TConcrete : TAbstract  {
            var resolver = new TransientObjectResolver<TAbstract, TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddSingleton<TConcrete>() {
            if (typeof(TConcrete).IsInterface)
                throw new ArgumentException($"{typeof(TConcrete)} cannot be an interface when added this way", nameof(TConcrete));
            
            var resolver = new SingletonObjectResolver<TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>()  where TConcrete : TAbstract {
            var resolver = new SingletonObjectResolver<TAbstract, TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IDIContainer Build() {
            foreach (var resolver in m_Resolvers) resolver.Resolve();

            return new DIContainer(m_Registry);
        }
    }
}