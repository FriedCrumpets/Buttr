namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrInstanceTemplate {
        private readonly string m_ProjectName;
        private readonly string m_Ns;
        private readonly string m_Name;
        private readonly PackageType m_Type;

        public ButtrInstanceTemplate(string projectName, string ns, string name, PackageType type) {
            m_ProjectName = projectName;
            m_Ns = ns;
            m_Name = name;
            m_Type = type;
        }

        public string Generate() {
            return m_Type switch {
                PackageType.UI => $@"using Buttr.Core;
using Buttr.Unity;
using UnityEngine;
using UnityEngine.UIElements;

namespace {m_Ns} {{
    [ DefaultExecutionOrder(-10000) ]
    public sealed class {m_Name}Instance : MonoBehaviour {{
        [Header(""Scriptable Objects"")]
        [SerializeField] private ScriptableInjector m_Injector;

        [Header(""References"")]
        [SerializeField] private UIDocument m_UIDocument;

        private IDIContainer m_Container;

        private void Awake() {{
            var builder = new ScopeBuilder({m_Name}Package.Scope);
            
            m_Injector.Inject(builder);

            builder.Use{m_Name}()
                .WithFactory<{m_Name}View>(() => new {m_Name}View(m_UIDocument));

            m_Container = builder.Build();
        }}

        private void OnDestroy() {{
            m_Container?.Dispose();
        }}
    }}
}}",
                _ => string.Empty
            };
        }
    }
}