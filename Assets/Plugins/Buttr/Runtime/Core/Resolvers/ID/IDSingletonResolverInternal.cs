using System;

namespace Buttr.Core {
    internal sealed class IDSingletonResolverInternal<TConcrete> : ObjectResolverBase<TConcrete> {
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;
        private bool m_IsResolved;

        private TConcrete m_Instance;
        
        public IDSingletonResolverInternal(Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }
        
        public override bool IsResolved {
            get => m_IsResolved;
        }

        public override object Resolve() {
            if (m_Instance != null) return m_Instance;
            
            var dependencies = ArrayPool<object>.Get(requirements.Length);
            if(requirements.Length > 0 ) {
                ApplicationRegistry.GetDependencies(requirements, dependencies);
                if (dependencies.TryValidate(requirements) == false) {
                    throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)})");
                }
            }

            m_Instance = m_Configuration(m_FactoryOverride == null
                ? factory(dependencies)
                : m_FactoryOverride());
            
            ArrayPool<object>.Release(dependencies);
            m_IsResolved = true;
            
            return m_Instance;
        }
    }
}