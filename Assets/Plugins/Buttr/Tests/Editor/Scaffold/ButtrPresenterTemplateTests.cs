using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrPresenterTemplateTests {
        [Test]
        public void Generate_ProducesCorrectClassName() {
            var result = new ButtrPresenterTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public sealed class InventoryPresenter"));
        }

        [Test]
        public void Generate_InjectsModelViaCtor() {
            var result = new ButtrPresenterTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public InventoryPresenter(InventoryModel model)"));
        }

        [Test]
        public void Generate_StoresModelAsReadonlyField() {
            var result = new ButtrPresenterTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("private readonly InventoryModel m_Model"));
        }
    }
}