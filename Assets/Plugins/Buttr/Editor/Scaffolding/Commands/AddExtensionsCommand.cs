namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddExtensionsCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddExtensionsCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            m_PackageFolder.WriteFileIfNew($"{name}Extensions.cs", new ButtrExtensionsTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}