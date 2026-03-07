namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddExtensionsCommand {
        private readonly string m_PackageFolder;

        public AddExtensionsCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            m_PackageFolder.WriteFileIfNew($"{name}Extensions.cs", new ButtrExtensionsTemplate(ns, name).Generate());
        }
    }
}