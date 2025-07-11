using System.Threading;
using Buttr.Core;
using UnityEngine;

namespace Buttr.Unity {
    public abstract class UnityApplicationLoaderBase : ScriptableObject {
        public abstract Awaitable LoadAsync(CancellationToken cancellationToken);
        public virtual Awaitable UnloadAsync() { return AwaitableUtility.CompletedTask; }
    }
}