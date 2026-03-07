namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrDefaultBehaviourTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrDefaultBehaviourTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    public sealed class Default{m_Name}Behaviour : I{m_Name}Behaviour {{
        public void Tick({m_Name}Context ctx) {{
        }}
    }}
}}
";
        }
    }
}