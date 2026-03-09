namespace Buttr.Editor.Scaffolding {
    internal readonly ref struct ButtrREADMETemplate {
        private readonly string m_PackageName;
        private readonly PackageType m_Type;

        public ButtrREADMETemplate(string packageName, PackageType type) {
            m_PackageName = packageName;
            m_Type = type;
        }

        public string Generate() {
            var typeLabel = m_Type == PackageType.Feature ? "Feature" : "Core";
            var usage = m_Type == PackageType.Feature
                ? $"var builder = new ScopeBuilder({m_PackageName}Package.Scope);\nbuilder.Use{m_PackageName}();"
                : $"builder.Use{m_PackageName}();";

            return $@"# {m_PackageName}

{typeLabel} package.

## Overview

<!-- Describe what this package does -->

## Dependencies

<!-- List any packages this depends on -->

## Usage

```csharp
{usage}
```
";
        }
    }
}