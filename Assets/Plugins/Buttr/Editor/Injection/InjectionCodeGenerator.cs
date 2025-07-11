using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Buttr.Editor.Injection {
    [InitializeOnLoad]
    internal static class InjectionCodeGenerator {
        static InjectionCodeGenerator() {
            EditorApplication.delayCall += Generate;
        }
        
        private static Dictionary<string, CachedEntry> s_Cache;

        private static void Generate() {
            var configuration = InjectionConfiguration.LoadOrDefault();
            s_Cache = configuration.LoadCache();
            var updatedCache = new Dictionary<string, CachedEntry>();
            var seenKeys = new HashSet<string>();
            
            var monoTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.FullName.StartsWith("Unity"))
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) && t.IsClass);

            if (Directory.Exists(configuration.GeneratedPath) == false) {
                Directory.CreateDirectory(configuration.GeneratedPath);
            }
            
            foreach (var type in monoTypes) {
                var injectFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(field => field.ConfirmInjectionAttributePresent())
                    .ToList();

                if (injectFields.Count == 0) continue;

                var key = type.FullName;
                var fieldSig = string.Join(";", injectFields.Select(field => $"{field.Name}:{field.FieldType.FullName}"));
                var hash = fieldSig.ComputeSHA1();

                seenKeys.Add(key);

                if (s_Cache.TryGetValue(key, out var entry) && entry.hash == hash) {
                    updatedCache[key] = entry; // no change to entry
                    continue;
                }
                
                var directory = Path.Combine(configuration.GeneratedPath, string.IsNullOrEmpty(type.Namespace) ? "Root" : Path.Combine(type.Namespace.Split('.')));
                var filePath = Path.Combine(directory, $"{type.Name}_Inject.g.cs");
                var code = (type, injectFields).GenerateCode();

                if (Directory.Exists(directory) == false)
                    Directory.CreateDirectory(directory);
                
                File.WriteAllText(filePath, code);
                updatedCache[key] = new CachedEntry { path = filePath, hash = hash };
                
                s_Cache.DeleteStaleFiles(seenKeys);
                s_Cache = updatedCache;
                configuration.SaveCache(s_Cache);
            }
        }
    }

}