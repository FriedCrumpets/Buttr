using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrServiceTemplateTests {
        [Test]
        public void Generate_ImplementsServiceContract() {
            var result = new ButtrServiceTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("IInventoryService"));
        }

        [Test]
        public void Generate_InjectsPresenterViaCtor() {
            var result = new ButtrServiceTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public InventoryService(InventoryPresenter presenter)"));
        }
    }
}