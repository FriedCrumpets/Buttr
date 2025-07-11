using System;
using System.Collections.Generic;

namespace Buttr.Core {
    internal sealed class SingletonObjectResolver<TConcrete> : IResolver, IConfigurable<TConcrete> {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;
    
        internal SingletonObjectResolver(Dictionary<Type, IObjectResolver> registry) {
            m_Registry = registry;
        }
        
        public void Resolve() {
            m_Registry[typeof(TConcrete)] = new SingletonResolverInternal<TConcrete>(m_Registry, m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Registry[typeof(TConcrete)].Resolve() is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }
    
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }
    
    internal sealed class SingletonObjectResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> where TConcrete : TAbstract {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;
    
        internal SingletonObjectResolver(Dictionary<Type, IObjectResolver> registry) {
            m_Registry = registry;
        }
        
        public void Resolve() {
            m_Registry[typeof(TAbstract)] = new SingletonResolverInternal<TConcrete>(m_Registry, m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Registry[typeof(TAbstract)].Resolve() is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }
    
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }
}