namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddPresenterCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddPresenterCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Components");
            folder.WriteFileIfNew($"{name}Presenter.cs", new ButtrPresenterTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}