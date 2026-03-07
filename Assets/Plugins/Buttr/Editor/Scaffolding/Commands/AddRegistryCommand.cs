namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddRegistryCommand {
        private readonly string m_PackageFolder;

        public AddRegistryCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var components = m_PackageFolder.EnsureSubFolder("Components");
            var identifiers = m_PackageFolder.EnsureSubFolder("Identifiers");
            var monoBehaviours = m_PackageFolder.EnsureSubFolder("MonoBehaviours");
            identifiers.WriteFileIfNew($"{name}Id.cs", new ButtrIdentifierTemplate(ns, name).Generate());
            monoBehaviours.WriteFileIfNew($"{name}Controller.cs", new ButtrControllerTemplate(ns, name).Generate());
            components.WriteFileIfNew($"{name}Registry.cs", new ButtrRegistryTemplate(ns, name).Generate());
        }
    }
}