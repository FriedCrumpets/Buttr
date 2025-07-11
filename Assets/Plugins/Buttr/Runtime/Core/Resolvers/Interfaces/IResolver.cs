using System;

namespace Buttr.Core {
    public interface IResolver : IDisposable {
        void Resolve();
    }
}