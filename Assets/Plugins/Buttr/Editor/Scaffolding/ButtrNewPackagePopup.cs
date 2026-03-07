using UnityEditor;
using UnityEngine;

namespace Buttr.Editor.Scaffolding {
    internal sealed class ButtrNewPackagePopup : EditorWindow {
        private string m_PackageName = string.Empty;
        private string m_ParentFolder;
        private PackageType m_Type;
        private PackageOptions m_Options;

        internal static void Show(string parentFolder, PackageType type) {
            var window = CreateInstance<ButtrNewPackagePopup>();
            window.m_ParentFolder = parentFolder;
            window.m_Type = type;
            window.titleContent = new GUIContent(type == PackageType.Feature ? "New Feature" : "New Core Package");
            window.minSize = new Vector2(300, 260);
            window.maxSize = new Vector2(300, 260);
            window.ShowUtility();
        }

        private void OnGUI() {
            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField("Package Name", EditorStyles.boldLabel);
            m_PackageName = EditorGUILayout.TextField(m_PackageName);

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Optional Additions", EditorStyles.boldLabel);

            ToggleOption(PackageOptions.Handlers, "Handlers");
            ToggleOption(PackageOptions.Behaviours, "Behaviours");
            ToggleOption(PackageOptions.Identifiers, "Identifiers");
            ToggleOption(PackageOptions.Configurations, "Configurations");
            ToggleOption(PackageOptions.Common, "Common");
            ToggleOption(PackageOptions.Exceptions, "Exceptions");

            EditorGUILayout.Space(12);

            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(m_PackageName));

            if (GUILayout.Button("Create Package", GUILayout.Height(28))) {
                ButtrPackageScaffolder.CreatePackage(m_ParentFolder, m_PackageName.Trim(), m_Type, m_Options);
                Close();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void ToggleOption(PackageOptions option, string label) {
            var active = m_Options.HasFlag(option);
            var toggled = EditorGUILayout.Toggle(label, active);

            if (toggled && false == active)
                m_Options |= option;
            else if (false == toggled && active)
                m_Options &= ~option;
        }
    }
}