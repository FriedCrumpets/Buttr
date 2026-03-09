using System;
using System.Collections.Generic;

namespace Buttr.Core {
    public sealed class ScopeBuilder : IDIBuilder {
        private readonly string m_Key;
        private readonly Dictionary<Type, IObjectResolver> m_Registry = new();
        private readonly HashSet<Type> m_HiddenTypes = new();

        private readonly IResolverCollection m_Resolvers;
        private readonly IResolverCollection m_Hidden;

        public ScopeBuilder(string key) {
            m_Key = key;
            m_Resolvers = new ObjectResolverCollection(m_Registry);
            m_Hidden = new HiddenObjectResolverCollection(m_Registry, m_HiddenTypes);
        }

        public IResolverCollection Resolvers => m_Resolvers;
        public IResolverCollection Hidden => m_Hidden;

        public IConfigurable<TConcrete> AddTransient<TConcrete>() {
            return m_Resolvers.AddTransient<TConcrete>();
        }

        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>()  where TConcrete : TAbstract  {
            return m_Resolvers.AddTransient<TAbstract, TConcrete>();
        }

        public IConfigurable<TConcrete> AddSingleton<TConcrete>() {
            return m_Resolvers.AddSingleton<TConcrete>();
        }

        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>()  where TConcrete : TAbstract {
            return m_Resolvers.AddSingleton<TAbstract, TConcrete>();
        }

        public IDIContainer Build() {
            m_Hidden.Resolve();
            m_Resolvers.Resolve();

            var container = new ScopeContainer(m_Registry, m_HiddenTypes);
            container.ScopeRegistration = ScopeRegistry.Register(m_Key, container);
            return container;
        }
    }
}
