namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddExceptionCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddExceptionCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Exceptions");
            folder.WriteFileIfNew($"{name}Exception.cs", new ButtrExceptionTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}