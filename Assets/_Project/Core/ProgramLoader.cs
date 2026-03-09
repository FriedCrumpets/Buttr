using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using UnityEngine;

namespace Buttr {
    [CreateAssetMenu(fileName = "ProgramLoader", menuName = "Buttr/Loaders/Program", order = 0)]
    public sealed class ProgramLoader : UnityApplicationLoaderBase {
        private ApplicationLifetime m_Lifetime;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            m_Lifetime = Program.Main();
            return AwaitableUtility.CompletedTask;
        }

        public override Awaitable UnloadAsync() {
            m_Lifetime?.Dispose();
            return AwaitableUtility.CompletedTask;
        }
    }
}
