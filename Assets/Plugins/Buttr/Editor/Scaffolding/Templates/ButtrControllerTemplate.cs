namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrControllerTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrControllerTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using Buttr.Unity;
using UnityEngine;

namespace {m_Ns} {{
    public sealed partial class {m_Name}Controller : MonoBehaviour {{
    }}
}}
";
        }
    }
}