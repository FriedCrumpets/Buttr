namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddControllerCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddControllerCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("MonoBehaviours");
            folder.WriteFileIfNew($"{name}Controller.cs", new ButtrControllerTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}