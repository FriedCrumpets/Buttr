using System;

namespace Buttr.Core {
    public sealed class ApplicationContainer : IDisposable {
        private readonly IDisposable m_Cleanup;
        
        public ApplicationContainer(IDisposable cleanup) {
            m_Cleanup = cleanup;
        }
        
        public void Dispose() {
            m_Cleanup.Dispose();
        }
    }
}