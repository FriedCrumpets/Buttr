namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddViewCommand {
        private readonly string m_PackageFolder;

        public AddViewCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("MonoBehaviours");
            folder.WriteFileIfNew($"{name}View.cs", new ButtrViewTemplate(ns, name).Generate());
        }
    }
}