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
        private readonly {m_Name}Model m_Model;

        public {m_Name}Service({m_Name}Model model) {{
            m_Model = model;
        }}
    }}
}}
";
        }
    }
}