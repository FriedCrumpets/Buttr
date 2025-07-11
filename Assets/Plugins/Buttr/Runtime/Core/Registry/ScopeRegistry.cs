using System;
using System.Collections.Generic;

namespace Buttr.Core {
    public static class ScopeRegistry {
        private static readonly Dictionary<string, IDIContainer> s_Scopes = new();

        internal static IDisposable Register(string key, IDIContainer container) {
            return new Registration(key, container);
        }

        public static IDIContainer Get(string key) {
            return s_Scopes[key];
        }

        public static void RemoveScope(string key) {
            s_Scopes.Remove(key);
        }

        public static void Clear() {
            s_Scopes.Clear();
        }

        private sealed class Registration : IDisposable {
            private readonly string m_Key;
            
            public Registration(string key, IDIContainer container) {
                s_Scopes.Add(key, container);
                m_Key = key;
            }
            
            public void Dispose() {
                s_Scopes.Remove(m_Key);
            }
        } 
    }
}