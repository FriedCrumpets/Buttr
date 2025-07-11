using System;

namespace Buttr.Core {
    public sealed class ApplicationBuilder : IContainerBuilder {
        private readonly IResolverCollection m_Resolvers = new ApplicationResolverCollection();
        private readonly IResolverCollection m_Hidden = new ApplicationHiddenCollection();

        private IDisposable m_Cleanup;

        public IResolverCollection Resolvers { get { return m_Resolvers; } }
        public IResolverCollection Hidden { get { return m_Hidden; } }
        
        public DisposableCollection Cleanup {
            set { m_Cleanup = value; }
        }
        
        public ApplicationLifetime Build() {
            m_Hidden.Resolve();
            m_Resolvers.Resolve();
            
            var cleanup = m_Cleanup is not null
                ? new DisposableCollection(new IDisposable[] { m_Cleanup, m_Resolvers, m_Hidden, new Disposable(ScopeRegistry.Clear) })
                : new DisposableCollection(new IDisposable[] { m_Resolvers, m_Hidden, new Disposable(ScopeRegistry.Clear) });

            return new ApplicationLifetime(cleanup);
        }
    }
}