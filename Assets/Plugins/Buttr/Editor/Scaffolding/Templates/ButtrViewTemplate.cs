namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrViewTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrViewTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using UnityEngine;

namespace {m_Ns} {{
    public sealed class {m_Name}View : MonoBehaviour {{
    }}
}}
";
        }
    }
}