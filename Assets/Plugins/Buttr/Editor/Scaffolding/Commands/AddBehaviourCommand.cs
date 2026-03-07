namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddBehaviourCommand {
        private readonly string m_PackageFolder;

        public AddBehaviourCommand(string packageFolder) {
            m_PackageFolder = packageFolder;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var behaviours = m_PackageFolder.EnsureSubFolder("Behaviours");
            var common = m_PackageFolder.EnsureSubFolder("Common");
            behaviours.WriteFileIfNew($"I{name}Behaviour.cs", new ButtrBehaviourInterfaceTemplate(ns, name).Generate());
            behaviours.WriteFileIfNew($"Default{name}Behaviour.cs", new ButtrDefaultBehaviourTemplate(ns, name).Generate());
            common.WriteFileIfNew($"{name}Context.cs", new ButtrContextTemplate(ns, name).Generate());
        }
    }
}