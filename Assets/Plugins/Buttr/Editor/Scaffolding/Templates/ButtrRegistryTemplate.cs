namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrRegistryTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrRegistryTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using System;
using System.Collections.Generic;

namespace {m_Ns} {{
    public sealed class {m_Name}Registry {{
        private readonly Dictionary<{m_Name}Id, {m_Name}Controller> m_Entries = new();

        public IReadOnlyCollection<{m_Name}Controller> Values => m_Entries.Values;
        public int Count => m_Entries.Count;

        public IDisposable Register({m_Name}Id id, {m_Name}Controller entry) {{
            m_Entries[id] = entry;
            return new Registration(this, id);
        }}

        public bool TryGet({m_Name}Id id, out {m_Name}Controller entry) {{
            return m_Entries.TryGetValue(id, out entry);
        }}

        public {m_Name}Controller Get({m_Name}Id id) {{
            return m_Entries[id];
        }}

        private void Deregister({m_Name}Id id) {{
            m_Entries.Remove(id);
        }}

        private sealed class Registration : IDisposable {{
            private readonly {m_Name}Registry m_Registry;
            private readonly {m_Name}Id m_Id;

            public Registration({m_Name}Registry registry, {m_Name}Id id) {{
                m_Registry = registry;
                m_Id = id;
            }}

            public void Dispose() {{
                m_Registry.Deregister(m_Id);
            }}
        }}
    }}
}}
";
        }
    }
}