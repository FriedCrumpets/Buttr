namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrPackageExtensionTemplate {
        private readonly string m_Ns;
        private readonly string m_Name;
        private readonly PackageType m_Type;

        public ButtrPackageExtensionTemplate(string ns, string name, PackageType type) {
            m_Ns = ns;
            m_Name = name;
            m_Type = type;
        }

        public string Generate() {
            return m_Type switch {
                PackageType.Feature => $@"using Buttr.Core;

namespace {m_Ns} {{
    public static class {m_Name}Package {{
        public const string Scope = ""{m_Name.ToLowerInvariant()}"";

        public static IConfigurableCollection Use{m_Name}(this ScopeBuilder builder) {{
            return new ConfigurableCollection()
                .Register(builder.AddSingleton<I{m_Name}Service, {m_Name}Service>())
                .Register(builder.AddSingleton<{m_Name}Model>())
                .Register(builder.AddSingleton<{m_Name}Presenter>())
                .Register(builder.AddSingleton<{m_Name}Mediator>());
        }}
    }}
}}
",
                PackageType.Core => $@"using Buttr.Core;

namespace {m_Ns} {{
    public static class {m_Name}Package {{
        public static IConfigurableCollection Use{m_Name}(this ApplicationBuilder builder) {{
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<I{m_Name}Service, {m_Name}Service>())
                .Register(builder.Resolvers.AddSingleton<{m_Name}Model>())
                .Register(builder.Resolvers.AddSingleton<{m_Name}Presenter>())
                .Register(builder.Resolvers.AddSingleton<{m_Name}Mediator>());
        }}
    }}
}}
",
                PackageType.UI => $@"using Buttr.Core;

namespace {m_Ns} {{
    public static class {m_Name}Package {{
        public const string Scope = ""{m_Name.ToLowerInvariant()}"";

        public static IConfigurableCollection Use{m_Name}(this ScopeBuilder builder) {{
            return new ConfigurableCollection()
                .Register(builder.Resolvers.AddSingleton<I{m_Name}Service, {m_Name}Service>())
                .Register(builder.Resolvers.AddSingleton<{m_Name}Model>())
                .Register(builder.Resolvers.AddSingleton<{m_Name}Presenter>())
                .Register(builder.Resolvers.AddSingleton<{m_Name}View>())
                .Register(builder.Resolvers.AddSingleton<{m_Name}Mediator>());
        }}
    }}
}}
",
                _ => string.Empty
            };
        }
    }
}