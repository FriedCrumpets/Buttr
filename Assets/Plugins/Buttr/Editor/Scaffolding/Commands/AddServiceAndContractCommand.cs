namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddServiceAndContractCommand {
        private readonly string m_PackageFolder;

        public AddServiceAndContractCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var components = m_PackageFolder.EnsureSubFolder("Components");
            var contracts = m_PackageFolder.EnsureSubFolder("Contracts");
            components.WriteFileIfNew($"{name}Service.cs", new ButtrServiceTemplate(ns, name).Generate());
            contracts.WriteFileIfNew($"I{name}Service.cs", new ButtrServiceContractTemplate(ns, name).Generate());
        }
    }
}