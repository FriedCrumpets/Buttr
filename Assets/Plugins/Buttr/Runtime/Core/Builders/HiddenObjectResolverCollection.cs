using System;
using System.Collections.Generic;

namespace Buttr.Core {
    internal sealed class HiddenObjectResolverCollection : IResolverCollection {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private readonly HashSet<Type> m_HiddenTypes;
        private readonly List<IResolver> m_Resolvers = new();

        internal HiddenObjectResolverCollection(Dictionary<Type, IObjectResolver> registry, HashSet<Type> hiddenTypes) {
            m_Registry = registry;
            m_HiddenTypes = hiddenTypes;
        }

        public IConfigurable<TConcrete> AddSingleton<TConcrete>() {
            m_HiddenTypes.Add(typeof(TConcrete));
            var resolver = new SingletonObjectResolver<TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract {
            m_HiddenTypes.Add(typeof(TAbstract));
            var resolver = new SingletonObjectResolver<TAbstract, TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddTransient<TConcrete>() {
            m_HiddenTypes.Add(typeof(TConcrete));
            var resolver = new TransientObjectResolver<TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract {
            m_HiddenTypes.Add(typeof(TAbstract));
            var resolver = new TransientObjectResolver<TAbstract, TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public void Resolve() {
            foreach (var resolver in m_Resolvers) resolver.Resolve();
        }

        public void Dispose() {
            foreach (var resolver in m_Resolvers) resolver.Dispose();
        }
    }
}
