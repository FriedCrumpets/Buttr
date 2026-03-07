namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddRepositoryCommand {
        private readonly string m_PackageFolder;

        public AddRepositoryCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var contracts = m_PackageFolder.EnsureSubFolder("Contracts");
            contracts.WriteFileIfNew($"I{name}Repository.cs", new ButtrRepositoryContractTemplate(ns, name).Generate());
        }
    }
}