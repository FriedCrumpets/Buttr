namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrServiceContractTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrServiceContractTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    public interface I{m_Name}Service {{
    }}
}}
";
        }
    }
}