namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrRepositoryContractTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;

        public ButtrRepositoryContractTemplate(string ns, string name) {
            m_Ns = ns;
            m_Name = name;
        }

        public string Generate() {
            return $@"using System;
using System.Collections.Generic;

namespace {m_Ns} {{
    public interface I{m_Name}Repository<in TKey, TData> {{
        ICollection<TData> RetrieveAll();
        IEnumerable<TData> RetrieveByCondition(Func<TData, bool> condition);
        void Create(TData entity);
        TData Read(TKey id);
        void Update(TData entity);
        bool Delete(TData entity);
        bool Delete(TKey id);
        void Clear();
    }}
}}
";
        }
    }
}