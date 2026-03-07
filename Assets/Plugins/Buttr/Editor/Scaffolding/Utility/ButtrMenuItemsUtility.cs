using System.IO;
using UnityEditor;
using UnityEngine;

namespace Buttr.Editor.Scaffolding {
    internal static class ButtrMenuItemsUtility {
        public static bool HasConventionStructure() {
            return File.Exists(
                Path.Combine(Application.dataPath, "_Project", $"{Application.productName}.asmdef")
            );
        }

        public static string GetSelectedFolder() {
            var selected = Selection.activeObject;

            if (selected == null)
                return Application.dataPath;

            var path = AssetDatabase.GetAssetPath(selected) ?? string.Empty;

            if (string.IsNullOrEmpty(path))
                return Application.dataPath;

            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            if (false == Path.IsPathRooted(path))
                path = Path.Combine(Application.dataPath, "..", path ?? string.Empty);

            return Path.GetFullPath(path ?? string.Empty);
        }

        public static bool IsInsidePackage() {
            return FindPackageRoot() != null;
        }

        public static string FindPackageRoot() {
            var folder = GetSelectedFolder();
            var projectAsmdef = Path.Combine(Application.dataPath, "_Project", "_Project.asmdef");

            while (false == string.IsNullOrEmpty(folder)) {
                var asmdefFiles = Directory.GetFiles(folder, "*.asmdef", SearchOption.TopDirectoryOnly);

                if (asmdefFiles.Length > 0) {
                    var isProjectRoot = false;

                    foreach (var file in asmdefFiles) {
                        if (Path.GetFullPath(file) == Path.GetFullPath(projectAsmdef)) {
                            isProjectRoot = true;
                            break;
                        }
                    }

                    if (false == isProjectRoot)
                        return folder;
                }

                if (Path.GetFileName(folder) == "_Project")
                    return null;

                var parent = Path.GetDirectoryName(folder);

                if (parent == folder) break;

                folder = parent;
            }

            return null;
        }

        public static PackageType InferPackageType() {
            var root = FindPackageRoot();

            if (root == null) return PackageType.Feature;

            var normalised = root.Replace('\\', '/');

            if (normalised.Contains("/Core/"))
                return PackageType.Core;

            return PackageType.Feature;
        }
    }
}