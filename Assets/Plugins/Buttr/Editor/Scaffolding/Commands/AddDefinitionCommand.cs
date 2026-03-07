namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddDefinitionCommand {
        private readonly string m_PackageFolder;

        public AddDefinitionCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var projectName = ButtrPackageScaffolderUtility.GetRootNamespace();
            var folder = m_PackageFolder.EnsureSubFolder("Definitions");
            folder.WriteFileIfNew($"{name}Definition.cs", new ButtrDefinitionTemplate(projectName, ns, name).Generate());
        }
    }
}