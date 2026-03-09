using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrExceptionTemplateTests {
        [Test]
        public void Generate_ExtendsException() {
            var result = new ButtrExceptionTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain(": Exception"));
        }

        [Test]
        public void Generate_HasBothConstructors() {
            var result = new ButtrExceptionTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public InventoryException(string message) : base(message)"));
            Assert.That(result, Does.Contain("public InventoryException(string message, Exception innerException)"));
        }
    }
}