namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddRepositoryCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddRepositoryCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var contracts = m_PackageFolder.EnsureSubFolder("Contracts");
            contracts.WriteFileIfNew($"I{name}Repository.cs", new ButtrRepositoryContractTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}