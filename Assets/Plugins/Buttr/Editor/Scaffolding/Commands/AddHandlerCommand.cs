namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddHandlerCommand {
        private readonly string m_PackageFolder;

        public AddHandlerCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Handlers");
            folder.WriteFileIfNew($"{name}Handler.cs", new ButtrHandlerTemplate(ns, name).Generate());
        }
    }
}