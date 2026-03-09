namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrPresenterTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrPresenterTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    public sealed class {m_Name}Presenter {{
        private readonly {m_Name}Model m_Model;

        public {m_Name}Presenter({m_Name}Model model) {{
            m_Model = model;
        }}
    }}
}}
";
        }
    }
}