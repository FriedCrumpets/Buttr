using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrPackageExtensionTemplateTests {
        [Test]
        public void Generate_Feature_UsesScopeBuilder() {
            var result = new ButtrPackageExtensionTemplate("MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("this ScopeBuilder builder"));
        }

        [Test]
        public void Generate_Feature_HasScopeConstant() {
            var result = new ButtrPackageExtensionTemplate("MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("public const string Scope = \"inventory\""));
        }

        [Test]
        public void Generate_Feature_RegistersCoreTypes() {
            var result = new ButtrPackageExtensionTemplate("MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("IInventoryService, InventoryService"));
            Assert.That(result, Does.Contain("InventoryModel"));
            Assert.That(result, Does.Contain("InventoryPresenter"));
            Assert.That(result, Does.Contain("InventoryMediator"));
        }

        [Test]
        public void Generate_Core_UsesApplicationBuilder() {
            var result = new ButtrPackageExtensionTemplate("MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("this ApplicationBuilder builder"));
        }

        [Test]
        public void Generate_Core_DoesNotHaveScopeConstant() {
            var result = new ButtrPackageExtensionTemplate("MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Not.Contain("const string Scope"));
        }

        [Test]
        public void Generate_Core_UsesResolversAccessor() {
            var result = new ButtrPackageExtensionTemplate("MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("builder.Resolvers.AddSingleton"));
        }
    }
}