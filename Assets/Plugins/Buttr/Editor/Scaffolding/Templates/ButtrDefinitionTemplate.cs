namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrDefinitionTemplate {
        private readonly string m_ProjectName;
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrDefinitionTemplate(string projectName, string ns, string name) {
            m_ProjectName = projectName;
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using UnityEngine;

namespace {m_Ns} {{
    [CreateAssetMenu(fileName = ""New{m_Name}Definition"", menuName = ""{m_ProjectName}/Definitions/{m_Name}"", order = 0)]
    public class {m_Name}Definition : ScriptableObject {{
    }}
}}
";
        }
    }
}