namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrHandlerTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrHandlerTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using UnityEngine;

namespace {m_Ns} {{
    public abstract class {m_Name}Handler : ScriptableObject {{

        // Below are some examples of potential handler methods => create, remove, update or delete as required
        public abstract void Activated();
        public abstract void Tick();
        public abstract void Deactivated();
        public abstract void Cancelled();
    }}
}}
";
        }
    }
}