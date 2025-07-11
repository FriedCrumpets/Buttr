using System;

namespace Buttr.Core {
    public sealed class ApplicationLifetime : IDisposable {
        private readonly IDisposable m_Cleanup;
        
        public ApplicationLifetime(IDisposable cleanup) {
            m_Cleanup = cleanup;
        }
        
        public void Dispose() {
            m_Cleanup.Dispose();
        }
    }
}