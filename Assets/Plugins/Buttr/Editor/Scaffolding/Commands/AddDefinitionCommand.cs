namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddDefinitionCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddDefinitionCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var projectName = ButtrPackageScaffolderUtility.GetRootNamespace();
            var folder = m_PackageFolder.EnsureSubFolder("Definitions");
            folder.WriteFileIfNew($"{name}Definition.cs", new ButtrDefinitionTemplate(projectName, ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}