using System;

namespace Buttr.Injection {
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InjectAttribute : Attribute {

        public InjectAttribute() { }
        
        public InjectAttribute(string scope) {
            Scope = scope;
        }
        
        public string Scope { get; }
    }
}
