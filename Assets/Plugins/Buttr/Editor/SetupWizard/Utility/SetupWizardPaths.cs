using System.IO;
using UnityEditor;

namespace Buttr.Editor.SetupWizard
{
    /// <summary>
    /// Resolves asset paths relative to the Buttr package root.
    /// Works whether Buttr is installed via UPM (Packages/), embedded in
    /// Assets/Plugins/, or placed anywhere else in the project.
    ///
    /// Uses <see cref="MonoScript"/> to find this script's own location,
    /// then navigates up to the package root.
    /// </summary>
    internal static class SetupWizardPaths
    {
        private static string s_PackageRoot;

        /// <summary>
        /// The root directory of the Buttr package (e.g. "Packages/com.friedcrumpets.buttr"
        /// or "Assets/Plugins/Buttr"). Cached after first resolution.
        /// </summary>
        internal static string PackageRoot
        {
            get
            {
                if (s_PackageRoot == null)
                    s_PackageRoot = ResolvePackageRoot();

                return s_PackageRoot;
            }
        }

        // ── Asset Paths ──────────────────────────────────────────────

        /// <summary>Path to SetupWizard.uxml</summary>
        internal static string SetupWizardUxml
            // => $"{PackageRoot}/Editor/SetupWizard/UXML/SetupWizard.uxml";
            => $"Assets/Plugins/Buttr/Editor/SetupWizard/UXML/SetupWizard.uxml";

        /// <summary>Path to FolderItem.uxml</summary>
        internal static string FolderItemUxml
            // => $"{PackageRoot}/Editor/SetupWizard/UXML/FolderItem.uxml";
            => $"Assets/Plugins/Buttr/Editor/SetupWizard/UXML/FolderItem.uxml";

        /// <summary>Path to Theme.uss</summary>
        internal static string ThemeUss
            // => $"{PackageRoot}/Editor/SetupWizard/USS/Theme.uss";
            => $"Assets/Plugins/Buttr/Editor/SetupWizard/USS/Theme.uss";

        // ── Resolution ───────────────────────────────────────────────

        private static string ResolvePackageRoot()
        {
            // Find this script's asset path via MonoScript.
            // MonoScript.FromScriptableObject/FromMonoBehaviour won't work here
            // since this is a static class, so we search by type name.
            var guids = AssetDatabase.FindAssets($"t:MonoScript {nameof(SetupWizardPaths)}");

            foreach (var guid in guids)
            {
                var scriptPath = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);

                if (script != null && script.GetClass() == typeof(SetupWizardPaths))
                {
                    // Script lives at: <root>/Editor/SetupWizard/Common/SetupWizardPaths.cs
                    // Walk up 4 directories to reach the package root.
                    var directory = Path.GetDirectoryName(scriptPath); // Common/
                    directory = Path.GetDirectoryName(directory);       // SetupWizard/
                    directory = Path.GetDirectoryName(directory);       // Editor/
                    directory = Path.GetDirectoryName(directory);       // Package root

                    // Normalise to forward slashes for Unity's asset database
                    return directory?.Replace('\\', '/');
                }
            }

            // Fallback — should never happen if the package is installed correctly
            UnityEngine.Debug.LogError(
                "[Buttr] Could not resolve package root. " +
                "Falling back to Assets/Plugins/Buttr.");

            return "Assets/Plugins/Buttr";
        }
    }
}