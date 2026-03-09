using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrREADMETemplateTests {
        [Test]
        public void Generate_Feature_ShowsScopeUsage() {
            var result = new ButtrREADMETemplate("Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("ScopeBuilder"));
            Assert.That(result, Does.Contain("InventoryPackage.Scope"));
        }

        [Test]
        public void Generate_Core_ShowsBuilderUsage() {
            var result = new ButtrREADMETemplate("Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("builder.UseAudio()"));
        }

        [Test]
        public void Generate_HasPackageName() {
            var result = new ButtrREADMETemplate("Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.StartWith("# Inventory"));
        }

        [Test]
        public void Generate_Feature_LabelledAsFeature() {
            var result = new ButtrREADMETemplate("Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("Feature package"));
        }

        [Test]
        public void Generate_Core_LabelledAsCore() {
            var result = new ButtrREADMETemplate("Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("Core package"));
        }
    }
}