using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrControllerTemplateTests {
        [Test]
        public void Generate_ExtendsMonoBehaviour() {
            var result = new ButtrControllerTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("MonoBehaviour"));
        }

        [Test]
        public void Generate_IsPartial() {
            var result = new ButtrControllerTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("partial class InventoryController"));
        }

        [Test]
        public void Generate_IsSealed() {
            var result = new ButtrControllerTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public sealed partial class"));
        }
    }
}