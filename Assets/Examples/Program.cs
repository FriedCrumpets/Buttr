using System.Collections.Generic;
using Examples;

namespace Buttr.Core {
    public static class Program {
        public static ApplicationContainer Main() => Main(CMDArgs.Read());

        private static ApplicationContainer Main(IDictionary<string, string> args) {
            var builder = new ApplicationBuilder();

            builder.Resolvers.AddTransient<ITestService, TestService>();
            builder.Resolvers.AddSingleton<ITestService2, TestService2>();
            builder.Resolvers.AddSingleton<TestService3>();
            
            return builder.Build();
        }
    }
}