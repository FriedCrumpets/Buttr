using System;
using System.Collections.Generic;

namespace Buttr.Core {
    public sealed class ScopeBuilder : IDIBuilder {
        private readonly string m_Key;
        
        private readonly Dictionary<Type, IObjectResolver> m_Registry = new();
        private readonly List<IResolver> m_Resolvers = new();

        public ScopeBuilder(string key) {
            m_Key = key;
        }
        
        public IConfigurable<TConcrete> AddTransient<TConcrete>() {
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

            var container = new ScopeContainer(m_Registry);
            container.ScopeRegistration = ScopeRegistry.Register(m_Key, container);
            return container;
        }
    }
}