using System;
using System.Collections.Generic;

namespace Buttr.Core {
    internal sealed class ObjectResolverCollection : IResolverCollection {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private readonly List<IResolver> m_Resolvers = new();

        internal ObjectResolverCollection(Dictionary<Type, IObjectResolver> registry) {
            m_Registry = registry;
        }

        public IConfigurable<TConcrete> AddSingleton<TConcrete>() {
            var resolver = new SingletonObjectResolver<TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract {
            var resolver = new SingletonObjectResolver<TAbstract, TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddTransient<TConcrete>() {
            var resolver = new TransientObjectResolver<TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract {
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
