using System;

namespace Buttr.Core {
    internal sealed class IDTransientResolverInternal<TConcrete> : ObjectResolverBase<TConcrete> {
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;
        private bool m_IsResolved;

        public IDTransientResolverInternal(Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }
        
        public override bool IsResolved {
            get => m_IsResolved;
        }

        public override object Resolve() {
            var dependencies = ArrayPool<object>.Get(requirements.Length);
            try {
                ApplicationRegistry.GetDependencies(requirements, dependencies);
                if(requirements.Length > 0 ) {
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
    }
}