namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddExceptionCommand {
        private readonly string m_PackageFolder;

        public AddExceptionCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var folder = m_PackageFolder.EnsureSubFolder("Exceptions");
            folder.WriteFileIfNew($"{name}Exception.cs", new ButtrExceptionTemplate(ns, name).Generate());
        }
    }
}