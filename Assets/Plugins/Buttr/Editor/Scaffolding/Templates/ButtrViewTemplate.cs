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
using UnityEngine.UIElements;

namespace {m_Ns} {{
    public sealed class {m_Name}View {{
        public {m_Name}View(UIDocument document){{

        }}
    }}
}}
";
        }
    }
}