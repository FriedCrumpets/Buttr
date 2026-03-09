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
        private readonly HashSet<Type> m_HiddenTypes = new();

        private readonly IResolverCollection m_Resolvers;
        private readonly IResolverCollection m_Hidden;

        public DIBuilder() {
            m_Resolvers = new ObjectResolverCollection(m_Registry);
            m_Hidden = new HiddenObjectResolverCollection(m_Registry, m_HiddenTypes);
        }

        public IResolverCollection Resolvers => m_Resolvers;
        public IResolverCollection Hidden => m_Hidden;

        public IConfigurable<TConcrete> AddTransient<TConcrete>() {
            if (typeof(TConcrete).IsInterface)
                throw new ArgumentException($"{typeof(TConcrete)} cannot be an interface when added this way", nameof(TConcrete));

            return m_Resolvers.AddTransient<TConcrete>();
        }

        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>()  where TConcrete : TAbstract  {
            return m_Resolvers.AddTransient<TAbstract, TConcrete>();
        }

        public IConfigurable<TConcrete> AddSingleton<TConcrete>() {
            if (typeof(TConcrete).IsInterface)
                throw new ArgumentException($"{typeof(TConcrete)} cannot be an interface when added this way", nameof(TConcrete));

            return m_Resolvers.AddSingleton<TConcrete>();
        }

        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>()  where TConcrete : TAbstract {
            return m_Resolvers.AddSingleton<TAbstract, TConcrete>();
        }

        public IDIContainer Build() {
            m_Hidden.Resolve();
            m_Resolvers.Resolve();

            return new DIContainer(m_Registry, m_HiddenTypes);
        }
    }
}
