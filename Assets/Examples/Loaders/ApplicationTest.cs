using System.Threading;
using Buttr.Unity;
using Examples;
using UnityEngine;

namespace Buttr.Core {
    // [CreateAssetMenu(fileName = "UnityContainer", menuName = "Buttr/Examples/Loaders/Container", order = 0)]
    public sealed class ApplicationTest : UnityApplicationLoaderBase {
        private ApplicationContainer m_App;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {
            var builder = new ApplicationBuilder();
            builder.Resolvers.AddTransient<ITestService, TestService>();
            builder.Resolvers.AddSingleton<ITestService2, TestService2>();
            builder.Resolvers.AddSingleton<TestService3>();
            
            m_App = builder.Build();
            return AwaitableUtility.CompletedTask;
        }

        public override Awaitable UnloadAsync() {
            m_App.Dispose();
            return AwaitableUtility.CompletedTask;
        }
    }
}