namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddViewCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddViewCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("MonoBehaviours");
            folder.WriteFileIfNew($"{name}View.cs", new ButtrViewTemplate(ns, name).Generate());
        }
    }
}