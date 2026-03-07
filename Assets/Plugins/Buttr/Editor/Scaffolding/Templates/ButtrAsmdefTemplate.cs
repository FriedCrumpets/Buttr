namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrAsmdefTemplate {
        private readonly string m_Ns;

        public ButtrAsmdefTemplate(string ns) {
            m_Ns = ns;
        }

        public string Generate() {
            return $@"{{
    ""name"": ""{m_Ns}"",
    ""rootNamespace"": ""{m_Ns}"",
    ""references"": [
        ""Buttr.Core"",
        ""Buttr.Unity""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";
        }
    }
}