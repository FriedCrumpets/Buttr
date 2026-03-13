namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrServiceTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrServiceTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    public sealed class {m_Name}Service : I{m_Name}Service {{
        private readonly {m_Name}Presenter m_Presenter;

        public {m_Name}Service({m_Name}Presenter presenter) {{
            m_Presenter = presenter;
        }}
    }}
}}
";
        }
    }
}