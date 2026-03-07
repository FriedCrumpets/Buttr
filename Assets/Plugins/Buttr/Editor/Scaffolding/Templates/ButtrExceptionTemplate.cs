namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrExceptionTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrExceptionTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using System;

namespace {m_Ns} {{
    public sealed class {m_Name}Exception : Exception {{
        public {m_Name}Exception(string message) : base(message) {{ }}
        public {m_Name}Exception(string message, Exception innerException) : base(message, innerException) {{ }}
    }}
}}
";
        }
    }
}