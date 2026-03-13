namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrMediatorTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrMediatorTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using System;

namespace {m_Ns} {{
    public sealed class {m_Name}Mediator : IDisposable {{
        private readonly {m_Name}Presenter m_Presenter;
        private readonly {m_Name}Model m_Model;

        public {m_Name}Mediator({m_Name}Presenter presenter, {m_Name}Model model) {{
            // todo: subscribe to model changes

            m_Presenter = presenter;
            m_Model = model;
        }}

        public void Dispose() {{
            // todo: unsubscribe from model changes
        }}
    }}
}}
";
        }
    }
}