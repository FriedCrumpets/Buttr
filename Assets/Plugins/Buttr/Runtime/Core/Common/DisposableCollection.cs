using System;

namespace Buttr.Core {
    public sealed class DisposableCollection : IDisposable {
        private readonly IDisposable[] m_Disposables;
        
        public DisposableCollection(params IDisposable[] disposables) {
            m_Disposables = disposables ?? throw new ArgumentNullException(nameof(disposables), $"Cannot pass a null array into a {typeof(DisposableCollection)}");
        }

        public void Dispose() {
            foreach(var disposable in m_Disposables) disposable?.Dispose();
        }
    }
}