namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddControllerCommand {
        private readonly string m_PackageFolder;

        public AddControllerCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("MonoBehaviours");
            folder.WriteFileIfNew($"{name}Controller.cs", new ButtrControllerTemplate(ns, name).Generate());
        }
    }
}