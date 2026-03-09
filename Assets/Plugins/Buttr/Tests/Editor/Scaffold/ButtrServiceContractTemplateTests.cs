using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrServiceContractTemplateTests {
        [Test]
        public void Generate_ProducesInterface() {
            var result = new ButtrServiceContractTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public interface IInventoryService"));
        }
    }
}