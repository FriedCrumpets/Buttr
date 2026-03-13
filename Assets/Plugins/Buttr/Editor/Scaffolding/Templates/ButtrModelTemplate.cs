namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrModelTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrModelTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    public sealed class {m_Name}Model {{ }}
}}
";
        }
    }
}