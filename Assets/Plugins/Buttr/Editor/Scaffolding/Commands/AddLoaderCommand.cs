using UnityEngine;

namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct AddLoaderCommand {
        private readonly string m_PackageFolder;
        private readonly PackageType m_Type;
        private readonly bool m_RefreshAssetDatabase;

        public AddLoaderCommand(string packageFolder, PackageType type, bool refreshAssetDatabase) {
            m_PackageFolder = packageFolder;
            m_Type = type;
            m_RefreshAssetDatabase = refreshAssetDatabase;
        }

        public void Execute() {
            var (ns, name) = m_PackageFolder.InferPackage();
            var projectName = ButtrPackageScaffolderUtility.GetRootNamespace();
            var folder = m_PackageFolder.EnsureSubFolder("Loaders");
            folder.WriteFileIfNew($"{name}Loader.cs", new ButtrLoaderTemplate(projectName, ns, name, m_Type).Generate(), m_RefreshAssetDatabase);
            
            var relative = m_PackageFolder.Replace(Application.dataPath.Replace('/', '\\'), "").TrimStart('/', '\\');
            $"{name}Loader".QueuePendingAsset($"Assets/{relative}/Loaders/{name}Loader.asset");
        }
    }
}