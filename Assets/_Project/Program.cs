using System.Collections.Generic;
using Buttr.Core;

namespace Buttr {
    public static class Program {
        public static ApplicationLifetime Main() => Main(CMDArgs.Read());

        private static ApplicationLifetime Main(IDictionary<string, string> args) {
            var builder = new ApplicationBuilder();

            // Register your packages here:
            // builder.UseMyFeature();

            return builder.Build();
        }
    }
}
