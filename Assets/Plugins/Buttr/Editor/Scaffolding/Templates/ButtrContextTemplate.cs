namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrContextTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrContextTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"namespace {m_Ns} {{
    public readonly struct {m_Name}Context {{
        private readonly float m_DeltaTime;

        public {m_Name}Context(float deltaTime) {{
            m_DeltaTime = deltaTime; // example contextual option, customise as required
        }}

        public float DeltaTime => m_DeltaTime;
    }}
}}
";
        }
    }
}