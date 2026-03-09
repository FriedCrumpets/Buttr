using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrLoaderTemplateTests {
        [Test]
        public void Generate_Feature_UsesScopeBuilder() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("new ScopeBuilder(InventoryPackage.Scope)"));
        }

        [Test]
        public void Generate_Feature_StoresScopeContainer() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("IDIContainer m_Container"));
        }

        [Test]
        public void Generate_Core_UsesApplicationBuilder() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("new ApplicationBuilder()"));
        }

        [Test]
        public void Generate_Core_StoresApplicationLifetime() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Core.Audio", "Audio", PackageType.Core).Generate();
            Assert.That(result, Does.Contain("ApplicationLifetime m_Lifetime"));
        }

        [Test]
        public void Generate_UsesProjectNameInMenuPath() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("menuName = \"MyGame/Loaders/Inventory\""));
        }

        [Test]
        public void Generate_CallsUsePackageExtension() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("builder.UseInventory()"));
        }

        [Test]
        public void Generate_DisposesOnUnload() {
            var result = new ButtrLoaderTemplate("MyGame", "MyGame.Features.Inventory", "Inventory", PackageType.Feature).Generate();
            Assert.That(result, Does.Contain("m_Container?.Dispose()"));
        }
    }
}