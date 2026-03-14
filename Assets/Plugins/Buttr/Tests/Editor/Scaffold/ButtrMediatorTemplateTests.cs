using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrMediatorTemplateTests {
        [Test]
        public void Generate_ImplementsIDisposable() {
            var result = new ButtrMediatorTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("IDisposable"));
        }

        [Test]
        public void Generate_InjectsPresenterViaCtor() {
            var result = new ButtrMediatorTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("InventoryPresenter presenter"));
        }

        [Test]
        public void Generate_HasDisposeMethod() {
            var result = new ButtrMediatorTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public void Dispose()"));
        }
    }
}