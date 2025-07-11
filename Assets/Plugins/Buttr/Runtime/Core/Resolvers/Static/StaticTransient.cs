using System;

namespace Buttr.Core {
    internal sealed class StaticTransient<TConcrete> : ObjectResolverBase<TConcrete>, IDisposable {
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;
        private bool m_IsResolved;
        
        internal StaticTransient(Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) : base() {
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
            ApplicationRegistry.Register<TConcrete>(this);
        }

        public override bool IsResolved {
            get => m_IsResolved;
        }
        
        public override object Resolve() {
            var dependencies = ArrayPool<object>.Get(requirements.Length);
            try {
                if(requirements.Length > 0 ) {
                    ApplicationRegistry.GetDependencies(requirements, dependencies);
                    if (dependencies.TryValidate(requirements) == false) {
                        throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)})");
                    }
                }

                m_IsResolved = true;

                return m_Configuration(m_FactoryOverride == null
                    ? factory(dependencies)
                    : m_FactoryOverride());
            }
            finally {
                ArrayPool<object>.Release(dependencies);
            }
        }
        
        public void Dispose() {
            ApplicationRegistry.Remove<TConcrete>();
        }
    }
    
    internal sealed class StaticTransient<TAbstract, TConcrete> : ObjectResolverBase<TConcrete>, IDisposable {
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;
        private bool m_IsResolved;

        internal StaticTransient(Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) : base() {
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
            ApplicationRegistry.Register<TAbstract>(this);
        }
        
        public override bool IsResolved {
            get => m_IsResolved;
        }

        public override object Resolve() {
            var dependencies = ArrayPool<object>.Get(requirements.Length);
            
            try {
                ApplicationRegistry.GetDependencies(requirements, dependencies);
                if (dependencies.TryValidate(requirements) == false) {
                    throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)})");
                }

                m_IsResolved = true;

                return m_Configuration(m_FactoryOverride == null
                    ? factory(dependencies)
                    : m_FactoryOverride());
            }
            finally {
                ArrayPool<object>.Release(dependencies);
            }
        }
        
        public void Dispose() {
            ApplicationRegistry.Remove<TAbstract>();
        }
    }
}