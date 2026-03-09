namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddConfigurationCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddConfigurationCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var projectName = ButtrPackageScaffolderUtility.GetRootNamespace();
            var folder = m_PackageFolder.EnsureSubFolder("Configurations");
            folder.WriteFileIfNew($"{name}Configuration.cs", new ButtrConfigurationTemplate(projectName, ns, name).Generate(), m_RefreshAssetDatabase);

            var catalogFolder = m_PackageFolder.FindCatalogFolder();

            if (catalogFolder != null) {
                catalogFolder.EnsureSubFolder(name);
                $"{name}Configuration".QueuePendingAsset($"Assets/_Project/Catalog/{name}/{name}Configuration.asset");
            }
        }
    }
}