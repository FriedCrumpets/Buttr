using System;
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
            
            var title = type switch {
                PackageType.Feature => "New Feature",
                PackageType.Core => "New Core",
                PackageType.UI => "New UI",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            window.titleContent = new GUIContent(title);
            window.minSize = new Vector2(300, 300);
            window.maxSize = new Vector2(300, 520);
            window.ShowUtility();
        }

        private void OnGUI() {
            EditorGUILayout.Space(8);

            EditorGUILayout.LabelField("Package Name", EditorStyles.boldLabel);
            m_PackageName = EditorGUILayout.TextField(m_PackageName);

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Optional Additions", EditorStyles.boldLabel);

            ToggleOption(PackageOptions.Definitions, "Definitions");
            ToggleOption(PackageOptions.Handlers, "Handlers");
            ToggleOption(PackageOptions.Behaviours, "Behaviours");
            ToggleOption(PackageOptions.Identifiers, "Identifiers");
            ToggleOption(PackageOptions.Controller, "Controller");
            ToggleOption(PackageOptions.Configurations, "Configurations");
            ToggleOption(PackageOptions.Common, "Common");
            ToggleOption(PackageOptions.Exceptions, "Exceptions");

            EditorGUILayout.Space(12);
            
            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(m_PackageName));
            
            if (GUILayout.Button("Create Package", GUILayout.Height(28))) {
                ButtrPackageScaffolder.CreatePackage(m_ParentFolder, Sanitise(m_PackageName).Trim(), m_Type, m_Options);
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
        
        // Source - https://stackoverflow.com/a/847251
        // Posted by Andre, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-03-13, License - CC BY-SA 3.0
        private static string Sanitise( string name )
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape( new string( System.IO.Path.GetInvalidFileNameChars() ) );
            string invalidRegStr = string.Format( @"([{0}]*\.+$)|([{0}]+)", invalidChars );

            return System.Text.RegularExpressions.Regex.Replace( name, invalidRegStr, "_" );
        }
    }
}