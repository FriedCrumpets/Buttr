namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddModelCommand {
        private readonly string m_PackageFolder;

        public AddModelCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Components");
            folder.WriteFileIfNew($"{name}Model.cs", new ButtrModelTemplate(ns, name).Generate());
        }
    }
}