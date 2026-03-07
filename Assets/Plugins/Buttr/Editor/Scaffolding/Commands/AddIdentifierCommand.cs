namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddIdentifierCommand {
        private readonly string m_PackageFolder;

        public AddIdentifierCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Identifiers");
            folder.WriteFileIfNew($"{name}Id.cs", new ButtrIdentifierTemplate(ns, name).Generate());
        }
    }
}