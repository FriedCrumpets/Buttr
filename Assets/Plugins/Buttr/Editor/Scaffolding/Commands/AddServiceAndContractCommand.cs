namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddServiceAndContractCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddServiceAndContractCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var components = m_PackageFolder.EnsureSubFolder("Components");
            var contracts = m_PackageFolder.EnsureSubFolder("Contracts");
            components.WriteFileIfNew($"{name}Service.cs", new ButtrServiceTemplate(ns, name).Generate());
            contracts.WriteFileIfNew($"I{name}Service.cs", new ButtrServiceContractTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}