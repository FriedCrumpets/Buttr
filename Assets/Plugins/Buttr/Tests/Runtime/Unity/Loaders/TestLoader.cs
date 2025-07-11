using System.Threading;
using Buttr.Core;
using UnityEngine;
using Buttr.Unity;

namespace Buttr.Tests.Editor.Unity {
    internal sealed class TestLoader : UnityApplicationLoaderBase {
        public bool IsLoaded { get; private set; }
        
        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            IsLoaded = true;
            return AwaitableUtility.CompletedTask;
        }

        public override Awaitable UnloadAsync() {
            IsLoaded = false;
            return AwaitableUtility.CompletedTask;
        }
    }
}