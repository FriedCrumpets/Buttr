using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Buttr.Injection;

namespace Buttr.Editor.Injection {
    internal static class InjectionCodeGeneratorUtility {
        public static bool ConfirmInjectionAttributePresent(this FieldInfo field) {
            return field.IsDefined(typeof(InjectAttribute), true) || field.IsDefined(typeof(InjectScopeAttribute), true);
        }

        public static string ComputeSHA1(this string input) {
            using var sha1 = SHA1.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha1.ComputeHash(bytes);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public static void DeleteStaleFiles(this Dictionary<string, CachedEntry> cache, HashSet<string> seenKeys) {
            foreach (var kvp in cache) {
                if (seenKeys.Contains(kvp.Key))
                    continue;
                
                if (File.Exists(kvp.Value.path)) {
                    File.Delete(kvp.Value.path);
                }
            }
        }
    }
}