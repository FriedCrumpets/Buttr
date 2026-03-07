using System.IO;
using UnityEditor;
using UnityEngine;

namespace Buttr.Editor.Scaffolding {
    /// <summary>
    /// Scaffolds full packages following Buttr conventions.
    /// All methods are synchronous and write directly to disk.
    /// </summary>
    internal static class ButtrPackageScaffolder {
        /// <summary>
        /// Scaffolds a full package with core structure and optional additions.
        /// Called by the popup window for "New Feature" and "New Core Package".
        /// </summary>
        internal static void CreatePackage(string parentFolder, string packageName, PackageType type, PackageOptions options) {
            var root = Path.Combine(parentFolder, packageName);

            if (Directory.Exists(root)) {
                Debug.LogWarning($"[Buttr] Package '{packageName}' already exists at {root}");
                return;
            }

            var ns = parentFolder.ResolveNamespace(packageName);
            var projectName = ButtrPackageScaffolderUtility.GetRootNamespace();

            Debug.Log($"[Buttr] Creating {type} package: {packageName}...");

            Directory.CreateDirectory(root);

            // ── Package root files ───────────────────────────────────
            root.WriteFile($"{packageName}Package.cs",
                new ButtrPackageExtensionTemplate(ns, packageName, type).Generate());

            root.WriteFile($"{ns}.asmdef",
                new ButtrAsmdefTemplate(ns).Generate());

            root.WriteFile("README.md",
                new ButtrREADMETemplate(packageName, type).Generate());

            // ── Components ───────────────────────────────────────────
            var components = root.CreateSubFolder("Components");
            components.WriteFile($"{packageName}Model.cs",
                new ButtrModelTemplate(ns, packageName).Generate());
            components.WriteFile($"{packageName}Presenter.cs",
                new ButtrPresenterTemplate(ns, packageName).Generate());
            components.WriteFile($"{packageName}Mediator.cs",
                new ButtrMediatorTemplate(ns, packageName).Generate());
            components.WriteFile($"{packageName}Service.cs",
                new ButtrServiceTemplate(ns, packageName).Generate());

            // ── Contracts ────────────────────────────────────────────
            var contracts = root.CreateSubFolder("Contracts");
            contracts.WriteFile($"I{packageName}Service.cs",
                new ButtrServiceContractTemplate(ns, packageName).Generate());

            // ── MonoBehaviours ────────────────────────────────────────
            var monoBehaviours = root.CreateSubFolder("MonoBehaviours");
            monoBehaviours.WriteFile($"{packageName}View.cs",
                new ButtrViewTemplate(ns, packageName).Generate());

            // ── Loaders ──────────────────────────────────────────────
            var loaders = root.CreateSubFolder("Loaders");
            loaders.WriteFile($"{packageName}Loader.cs",
                new ButtrLoaderTemplate(projectName, ns, packageName, type).Generate());

            var relative = root.Replace(Application.dataPath, "").TrimStart('/', '\\');
            $"{packageName}Loader".QueuePendingAsset($"Assets/{relative}/Loaders/{packageName}Loader.asset");

            // ── Optional: Handlers ───────────────────────────────────
            if (options.HasFlag(PackageOptions.Handlers)) {
                var folder = root.CreateSubFolder("Handlers");
                folder.WriteFile($"{packageName}Handler.cs",
                    new ButtrHandlerTemplate(ns, packageName).Generate());
            }

            // ── Optional: Behaviours ─────────────────────────────────
            if (options.HasFlag(PackageOptions.Behaviours)) {
                var behaviours = root.CreateSubFolder("Behaviours");
                var common = root.CreateSubFolder("Common");
                behaviours.WriteFile($"I{packageName}Behaviour.cs",
                    new ButtrBehaviourInterfaceTemplate(ns, packageName).Generate());
                behaviours.WriteFile($"Default{packageName}Behaviour.cs",
                    new ButtrDefaultBehaviourTemplate(ns, packageName).Generate());
                common.WriteFile($"{packageName}Context.cs",
                    new ButtrContextTemplate(ns, packageName).Generate());
            }

            // ── Optional: Identifiers ────────────────────────────────
            if (options.HasFlag(PackageOptions.Identifiers)) {
                var folder = root.CreateSubFolder("Identifiers");
                folder.WriteFile($"{packageName}Id.cs",
                    new ButtrIdentifierTemplate(ns, packageName).Generate());
            }

            // ── Optional: Configurations ─────────────────────────────
            if (options.HasFlag(PackageOptions.Configurations)) {
                var folder = root.CreateSubFolder("Configurations");
                folder.WriteFile($"{packageName}Configuration.cs",
                    new ButtrConfigurationTemplate(projectName, ns, packageName).Generate());

                var catalogFolder = parentFolder.FindCatalogFolder();

                if (catalogFolder != null) {
                    catalogFolder.EnsureSubFolder(packageName);
                    $"{packageName}Configuration".QueuePendingAsset(
                        $"Assets/_Project/Catalog/{packageName}/{packageName}Configuration.asset");
                }
            }

            // ── Optional: Common ─────────────────────────────────────
            if (options.HasFlag(PackageOptions.Common) && false == options.HasFlag(PackageOptions.Behaviours))
                root.CreateSubFolder("Common");

            // ── Optional: Exceptions ─────────────────────────────────
            if (options.HasFlag(PackageOptions.Exceptions)) {
                var folder = root.CreateSubFolder("Exceptions");
                folder.WriteFile($"{packageName}Exception.cs",
                    new ButtrExceptionTemplate(ns, packageName).Generate());
            }

            AssetDatabase.Refresh();
            Debug.Log($"[Buttr] {type} package '{packageName}' created successfully.");
        }
    }
}