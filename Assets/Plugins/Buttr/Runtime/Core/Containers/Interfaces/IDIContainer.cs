using System;

namespace Buttr.Core {
    public interface IDIContainer<in TID> : IDisposable {
        Type Type { get; }

        T Get<T>(TID id);
        bool TryGet<T>(TID id, out T value);
    }
    
    public interface IDIContainer : IDisposable {
        T Get<T>();
        bool TryGet<T>(out T value);
    }
}