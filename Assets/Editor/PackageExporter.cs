using System.IO;
using UnityEditor;
using UnityEngine;

namespace Buttr.Editor {
    public static class PackageExporter {
        public static void CreatePackage() {
            var projectContent = new[] {
                "Assets/Plugins/Buttr", // <<< IMPORTANT: Change this to the path of your folder(s) to export
            };

            var packageName = "Buttr.unitypackage"; // <<< IMPORTANT: Desired name for your package file
            var outputPath = Path.Combine(Application.dataPath, "..", packageName); // Path to save outside Assets

            AssetDatabase.ExportPackage(
                projectContent,
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );

            Debug.Log($"Successfully exported Unity package to: {outputPath}");
        }
    }
}