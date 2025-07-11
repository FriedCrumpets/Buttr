using System.Collections.Generic;
using UnityEngine;

namespace Buttr.Core {
    public static class Program {
        public static ApplicationLifetime Main() => Main(CMDArgs.Read());

        private static ApplicationLifetime Main(IDictionary<string, string> args) {
            var builder = new ApplicationBuilder();

            Debug.Log("Built");
            
            return builder.Build();
        }
    }
}