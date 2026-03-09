using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrModelTemplateTests {
        [Test]
        public void Generate_ProducesCorrectClassName() {
            var result = new ButtrModelTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public sealed class InventoryModel"));
        }

        [Test]
        public void Generate_ProducesCorrectNamespace() {
            var result = new ButtrModelTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("namespace MyGame.Features.Inventory"));
        }
    }
}