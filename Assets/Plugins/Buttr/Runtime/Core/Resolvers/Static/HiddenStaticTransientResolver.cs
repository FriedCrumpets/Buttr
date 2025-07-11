using System;

namespace Buttr.Core {
    internal sealed class HiddenStaticTransientResolver<TConcrete> : IResolver, IConfigurable<TConcrete> {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;

        private StaticTransient<TConcrete> m_Transient;
        
        public void Resolve() {
            m_Transient = new StaticTransient<TConcrete>(m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Transient == null) return;
            
            m_Transient.Dispose();
            m_Transient = null;
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
    
    internal sealed class HiddenStaticTransientResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;

        private StaticTransient<TAbstract, TConcrete> m_Transient;
        
        public void Resolve() {
            m_Transient = new StaticTransient<TAbstract, TConcrete>(m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Transient == null) return;
            
            m_Transient.Dispose();
            m_Transient = null;
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