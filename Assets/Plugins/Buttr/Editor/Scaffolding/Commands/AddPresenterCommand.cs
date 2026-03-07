namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddPresenterCommand {
        private readonly string m_PackageFolder;

        public AddPresenterCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Components");
            folder.WriteFileIfNew($"{name}Presenter.cs", new ButtrPresenterTemplate(ns, name).Generate());
        }
    }
}