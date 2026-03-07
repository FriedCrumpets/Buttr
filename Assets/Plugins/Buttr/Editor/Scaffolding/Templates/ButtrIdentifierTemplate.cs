namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrIdentifierTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrIdentifierTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using System;

namespace {m_Ns} {{
    public readonly struct {m_Name}Id : IEquatable<{m_Name}Id> {{
        private readonly string m_Value;

        public {m_Name}Id(string value) {{
            m_Value = value;
        }}

        public string Value => m_Value;

        public bool Equals({m_Name}Id other) => Value == other.Value;
        public override bool Equals(object obj) => obj is {m_Name}Id other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value ?? string.Empty;

        public static implicit operator string({m_Name}Id entity) => entity.Value;
        public static implicit operator {m_Name}Id(string id) => new(id);

        public static bool operator ==({m_Name}Id left, {m_Name}Id right) => left.Equals(right);
        public static bool operator !=({m_Name}Id left, {m_Name}Id right) => !left.Equals(right);
    }}
}}
";
        }
    }
}