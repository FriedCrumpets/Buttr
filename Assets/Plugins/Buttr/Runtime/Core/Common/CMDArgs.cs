using System;
using System.Collections.Generic;
using UnityEngine;

namespace Buttr.Core {
    /// <summary>
    /// A simple static class for accessing environment arguments
    /// </summary>
    /// <remarks>
    /// Arguments are returned in a Key Value Pair IDictionary. If your application does not use this standard you will need a different
    /// Environmet Argument Reader
    /// </remarks>
    public static class CMDArgs {
        private static IDictionary<string, string> s_Args;
            
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Main() {
            s_Args = new Dictionary<string, string>(
                Environment
                    .GetCommandLineArgs()
                    .Read()
            );
        }
    
        public static IDictionary<string, string> Read() {
            return s_Args;
        }
    
        public static bool Exists(string arg) {
            return s_Args.ContainsKey(arg);
        }
            
        public static bool TryGetValue(string key, out string value) {
            value = !s_Args.TryGetValue(key, out var arg) ? default : arg;
            return s_Args.ContainsKey(key);
        }
            
        private static IEnumerable<KeyValuePair<string, string>> Read(this IReadOnlyList<string> args) {
            for (var i = 0; i < args.Count; i++) {
                var key = args[i];
                var value = ++i > args.Count - 1 ? string.Empty : args[i];
                yield return new KeyValuePair<string, string>(key, value);
            }
        }
    }
}