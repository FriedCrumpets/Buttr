using System.Collections.Generic;

namespace Buttr.Core {
    public sealed class ApplicationResolverCollection : IResolverCollection {
        private readonly List<IResolver> m_Resolvers = new();
        
        public IConfigurable<TConcrete> AddSingleton<TConcrete>() {
            var resolver = new StaticSingletonResolver<TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract {
            var resolver = new StaticSingletonResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddTransient<TConcrete>() {
            var resolver = new StaticTransientResolver<TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract {
            var resolver = new StaticTransientResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public void Resolve() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();
        }

        public void Dispose() {
            foreach(var resolver in m_Resolvers) resolver.Dispose();
        }
    }
}