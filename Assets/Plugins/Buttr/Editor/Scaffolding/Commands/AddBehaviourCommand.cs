namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddBehaviourCommand {
        private readonly string m_PackageFolder;
        private readonly bool m_RefreshAssetDatabase;

        public AddBehaviourCommand(string packageFolder, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var contracts = m_PackageFolder.EnsureSubFolder("Contracts");
            var behaviours = m_PackageFolder.EnsureSubFolder("Behaviours");
            var common = m_PackageFolder.EnsureSubFolder("Common");
            
            contracts.WriteFileIfNew($"I{name}Behaviour.cs", new ButtrBehaviourInterfaceTemplate(ns, name).Generate());
            behaviours.WriteFileIfNew($"Default{name}Behaviour.cs", new ButtrDefaultBehaviourTemplate(ns, name).Generate());
            common.WriteFileIfNew($"{name}Context.cs", new ButtrContextTemplate(ns, name).Generate(), m_RefreshAssetDatabase);
        }
    }
}