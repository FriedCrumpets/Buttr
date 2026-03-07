namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrExtensionsTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrExtensionsTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    internal static class {m_Name}Extensions {{
    }}
}}
";
        }
    }
}