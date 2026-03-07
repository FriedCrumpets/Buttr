namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddMediatorCommand {
        private readonly string m_PackageFolder;

        public AddMediatorCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Components");
            folder.WriteFileIfNew($"{name}Mediator.cs", new ButtrMediatorTemplate(ns, name).Generate());
        }
    }
}