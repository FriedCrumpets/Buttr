namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddMediatorCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddMediatorCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Components");
            folder.WriteFileIfNew($"{name}Mediator.cs", new ButtrMediatorTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}