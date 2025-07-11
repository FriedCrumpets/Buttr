using System;
using System.Collections.Generic;

namespace Buttr.Core {
    internal sealed class SingletonResolverInternal<TConcrete> : ObjectResolverBase<TConcrete> {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;
        private bool m_IsResolved;

        private TConcrete m_Instance;

        public SingletonResolverInternal(Dictionary<Type, IObjectResolver> registry, Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Registry = registry;
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }

        public override bool IsResolved {
            get => m_IsResolved;
        }

        public override object Resolve() {
            if (m_Instance != null) return m_Instance;

            var resolved = ArrayPool<object>.Get(requirements.Length);
            if (requirements.Length > 0) {
                m_Registry.TryResolve<TConcrete>(requirements, resolved);
            }

            m_Instance = m_Configuration(
                m_FactoryOverride == null
                    ? factory(resolved)
                    : m_FactoryOverride()
            );

            m_IsResolved = true;
            ArrayPool<object>.Release(resolved);
            
            return m_Instance;
        }
    }
}