using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrViewTemplateTests {
        [Test]
        public void Generate_ExtendsMonoBehaviour() {
            var result = new ButtrViewTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("MonoBehaviour"));
        }

        [Test]
        public void Generate_IsSealed() {
            var result = new ButtrViewTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public sealed class InventoryView"));
        }
    }
}