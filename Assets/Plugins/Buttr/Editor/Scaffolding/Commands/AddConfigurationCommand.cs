namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddConfigurationCommand {
        private readonly string m_PackageFolder;

        public AddConfigurationCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var projectName = ButtrPackageScaffolderUtility.GetRootNamespace();
            var folder = m_PackageFolder.EnsureSubFolder("Configurations");
            folder.WriteFileIfNew($"{name}Configuration.cs", new ButtrConfigurationTemplate(projectName, ns, name).Generate());

            var catalogFolder = m_PackageFolder.FindCatalogFolder();

            if (catalogFolder != null) {
                catalogFolder.EnsureSubFolder(name);
                $"{name}Configuration".QueuePendingAsset($"Assets/_Project/Catalog/{name}/{name}Configuration.asset");
            }
        }
    }
}