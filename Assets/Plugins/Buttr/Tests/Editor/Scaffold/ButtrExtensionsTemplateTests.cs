using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrExtensionsTemplateTests {
        [Test]
        public void Generate_IsInternalStatic() {
            var result = new ButtrExtensionsTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("internal static class InventoryExtensions"));
        }
    }
}