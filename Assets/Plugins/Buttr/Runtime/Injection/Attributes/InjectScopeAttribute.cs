using System;

namespace Buttr.Injection {
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectScopeAttribute : Attribute {
        public string Scope { get; }

        public InjectScopeAttribute(string scope) {
            if (string.IsNullOrEmpty(scope)) throw new InjectionException($"{typeof(InjectScopeAttribute)} must be given a string on construction");
            Scope = scope;
        }
    }
}