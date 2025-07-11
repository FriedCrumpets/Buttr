using System.Threading;
using Buttr.Unity;
using Examples;
using UnityEngine;

namespace Buttr.Core {
    [CreateAssetMenu(fileName = "UnityContainer", menuName = "Buttr/Examples/Loaders/Container", order = 0)]
    public sealed class ApplicationContainer : UnityApplicationLoaderBase {
        private ApplicationLifetime m_App;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();
            builder.Resolvers.AddSingleton<ITestService, TestService>();
            builder.Resolvers.AddSingleton<TestService>();
            
            m_App = builder.Build();
            return AwaitableUtility.CompletedTask;
        }

        public override Awaitable UnloadAsync() {
            m_App.Dispose();
            return AwaitableUtility.CompletedTask;
        }
    }
}