namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddSystemCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddSystemCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var components = m_PackageFolder.EnsureSubFolder("Components");
            var common = m_PackageFolder.EnsureSubFolder("Common");
            components.WriteFileIfNew($"{name}System.cs", new ButtrSystemTemplate(ns, name).Generate());
            common.WriteFileIfNew($"{name}Context.cs", new ButtrContextTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}