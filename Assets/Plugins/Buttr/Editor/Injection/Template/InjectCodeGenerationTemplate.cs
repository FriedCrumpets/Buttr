using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Buttr.Injection;

namespace Buttr.Editor.Injection {
    internal static class InjectCodeGenerationTemplate {
        public static string GenerateCode(this (Type type, List<FieldInfo> fields) info) {
            var ns = info.type.Namespace;
            var className = info.type.Name;
            var sb = new StringBuilder();

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using Buttr.Injection;");
            sb.AppendLine("using Buttr.Core;");
            sb.AppendLine();
            
            if (string.IsNullOrEmpty(ns) == false)
                sb.AppendLine($"namespace {ns} {{");

            sb.AppendLine($"    public partial class {className} : IInjectable {{");
            sb.AppendLine("        bool IInjectable.Injected { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        [RuntimeInitializeOnLoadMethod]");
            sb.AppendLine($"        static void __RegisterInjectionHandler() => InjectionProcessor.Register<{className}>(Inject);");
            sb.AppendLine();
            sb.AppendLine($"        private static void Inject({className} instance) {{");

            var scopes = (
                from f in info.fields 
                where f.IsDefined(typeof(InjectScopeAttribute), true) 
                select (InjectScopeAttribute)f.GetCustomAttribute(typeof(InjectScopeAttribute), true) 
                into attr select attr.Scope
            ).Distinct().ToList();

            foreach (var scope in scopes) {
                sb.AppendLine($"            var {scope} = ScopeRegistry.Get(\"{scope}\");");
            }
            
            foreach (var f in info.fields) {
                if (f.IsDefined(typeof(InjectScopeAttribute), true)) {
                    var attr = (InjectScopeAttribute)f.GetCustomAttribute(typeof(InjectScopeAttribute), true);
                    var scope = string.IsNullOrEmpty(attr.Scope) ? "\"\"" : $"{attr.Scope}";
                    sb.AppendLine($"            instance.{f.Name} = {scope}.Get<{f.FieldType.FullName}>();");
                }
                else {
                    sb.AppendLine($"            instance.{f.Name} = Application<{f.FieldType.FullName}>.Get();");
                }
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");

            if (string.IsNullOrEmpty(ns) == false) sb.AppendLine("}");

            return sb.ToString();
        }
    }
}