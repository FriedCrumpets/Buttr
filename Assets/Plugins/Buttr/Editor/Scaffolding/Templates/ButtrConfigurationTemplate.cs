namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrConfigurationTemplate {
        private readonly string m_ProjectName;
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrConfigurationTemplate(string projectName, string ns, string name) {
            m_ProjectName = projectName;
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using UnityEngine;

namespace {m_Ns} {{
    [CreateAssetMenu(fileName = ""{m_Name}Configuration"", menuName = ""{m_ProjectName}/Configurations/{m_Name}"", order = 0)]
    public sealed class {m_Name}Configuration : ScriptableObject {{ }}
}}
";
        }
    }
}