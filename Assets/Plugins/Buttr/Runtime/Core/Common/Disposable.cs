using System;

namespace Buttr.Core {
    internal sealed class Disposable : IDisposable {
        private readonly Action m_Disposal;
        
        public Disposable(Action disposal) {
            m_Disposal = disposal;
        }
        
        public void Dispose() {
            m_Disposal?.Invoke();
        }
    }
}