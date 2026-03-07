namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrLoaderTemplate {
        private readonly string m_ProjectName;
        private readonly string m_Ns;
        private readonly string m_Name;
        private readonly PackageType m_Type;

        public ButtrLoaderTemplate(string projectName, string ns, string name, PackageType type) {
            m_ProjectName = projectName;
            m_Ns = ns;
            m_Name = name;
            m_Type = type;
        }

        public string Generate() {
            return m_Type switch {
                PackageType.Feature => $@"using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using UnityEngine;

namespace {m_Ns} {{
    [CreateAssetMenu(fileName = ""{m_Name}Loader"", menuName = ""{m_ProjectName}/Loaders/{m_Name}"", order = 0)]
    public sealed class {m_Name}Loader : UnityApplicationLoaderBase {{
        private IDIContainer m_Container;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {{
            var builder = new ScopeBuilder({m_Name}Package.Scope);
            builder.Use{m_Name}();
            m_Container = builder.Build();
            return AwaitableUtility.CompletedTask;
        }}

        public override Awaitable UnloadAsync() {{
            m_Container?.Dispose();
            return AwaitableUtility.CompletedTask;
        }}
    }}
}}
",
                PackageType.Core => $@"using System.Threading;
using Buttr.Core;
using Buttr.Unity;
using UnityEngine;

namespace {m_Ns} {{
    [CreateAssetMenu(fileName = ""{m_Name}Loader"", menuName = ""{m_ProjectName}/Loaders/{m_Name}"", order = 0)]
    public sealed class {m_Name}Loader : UnityApplicationLoaderBase {{
        private ApplicationLifetime m_Lifetime;

        public override Awaitable LoadAsync(CancellationToken cancellationToken) {{
            var builder = new ApplicationBuilder();
            builder.Use{m_Name}();
            m_Lifetime = builder.Build();
            return AwaitableUtility.CompletedTask;
        }}

        public override Awaitable UnloadAsync() {{
            m_Lifetime?.Dispose();
            return AwaitableUtility.CompletedTask;
        }}
    }}
}}
",
                _ => string.Empty
            };
        }
    }
}