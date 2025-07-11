using System.Collections.Generic;

namespace Buttr.Core {
    /// <summary>
    /// All objects resolved will be unavailable statically, but will be available for injection through DI
    /// </summary>
    /// <remarks>
    /// This is important for Design, some of your services, controllers, factories... etc. you will not want to be available statically, but will require
    /// for DI injection. Due to this you can build wide object hierarchies without concerning yourself with issues of access. Of course anything can inject anything
    /// it's important to be aware of this and design your objects to be internal when necessary
    /// </remarks>
    public sealed class ApplicationHiddenCollection : IResolverCollection {
        private readonly List<IResolver> m_Resolvers = new();

        public IConfigurable<TConcrete> AddTransient<TConcrete>() {
            var resolver = new HiddenStaticTransientResolver<TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract {
            var resolver = new HiddenStaticTransientResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddSingleton<TConcrete>() {
            var resolver = new HiddenStaticSingletonResolver<TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract {
            var resolver = new HiddenStaticSingletonResolver<TAbstract, TConcrete>();
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