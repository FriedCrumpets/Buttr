using System.IO;
using UnityEngine;
using UnityEditor;

// ReSharper disable AssignNullToNotNullAttribute

namespace Buttr.Editor.Injection {
    [CreateAssetMenu(fileName = "ButtrInjectionConfiguration", menuName = "Buttr/Configurations/Injection", order = 0)]
    internal sealed class InjectionConfiguration : ScriptableObject {
        [field: SerializeField] public string GeneratedPath { get; private set; } = Path.Combine("Assets", "Buttr", "Injection");
        [field: SerializeField] public string CachePath { get; private set; } = Path.Combine("Library", "Buttr", "injection_cache.json");

        [ContextMenu("Reset To Default")]
        public void ResetToDefault() {
            GeneratedPath = "Assets/Buttr/Injection";
            CachePath = "Library/Buttr/injection_cache.json";
        }

        [ContextMenu("Clear Cache")]
        public void ClearCache() {
            if (File.Exists(CachePath)) {
                File.Delete(CachePath);
            }
        }

        private const string CONFIG_PATH = "Assets/Buttr/InjectionConfiguration";   
        
        public static InjectionConfiguration LoadOrDefault() {
            var configAssets = AssetDatabase.FindAssets("t:InjectionConfiguration");

            if(configAssets.Length > 0) {
                if (configAssets.Length > 1) {
                    Debug.LogWarning("Multiple InjectionConfiguration assets found. Using the first one found.");
                }
                
                var path = AssetDatabase.GUIDToAssetPath(configAssets[0]);
                return AssetDatabase.LoadAssetAtPath<InjectionConfiguration>(path);
            }
            
            var config = CreateInstance<InjectionConfiguration>();
            var directory = Path.GetDirectoryName(CONFIG_PATH);
            if(Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
            
            AssetDatabase.CreateAsset(config, $"{CONFIG_PATH}.asset");
            AssetDatabase.SaveAssets();

            Debug.Log($"Created default InjectionConfiguration at {CONFIG_PATH}");
            
            return config;
        }
    }
}