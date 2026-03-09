namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrBehaviourInterfaceTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrBehaviourInterfaceTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    public interface I{m_Name}Behaviour {{
        void Tick({m_Name}Context ctx);
    }}
}}
";
        }
    }
}