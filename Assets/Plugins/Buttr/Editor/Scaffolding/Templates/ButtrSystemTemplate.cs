namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrSystemTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrSystemTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    public sealed class {m_Name}System {{
        private I{m_Name}Behaviour m_ActiveBehaviour;

        public I{m_Name}Behaviour ActiveBehaviour {{
            get => m_ActiveBehaviour;
            set => m_ActiveBehaviour = value;
        }}

        public void Tick({m_Name}Context ctx) {{
        }}
    }}
}}
";
        }
    }
}