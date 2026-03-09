using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrIdentifierTemplateTests {
        [Test]
        public void Generate_IsReadonlyStruct() {
            var result = new ButtrIdentifierTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public readonly struct InventoryId"));
        }

        [Test]
        public void Generate_ImplementsIEquatable() {
            var result = new ButtrIdentifierTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("IEquatable<InventoryId>"));
        }

        [Test]
        public void Generate_HasImplicitStringOperator() {
            var result = new ButtrIdentifierTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public static implicit operator string(InventoryId entity)"));
        }

        [Test]
        public void Generate_HasImplicitIdOperator() {
            var result = new ButtrIdentifierTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public static implicit operator InventoryId(string id)"));
        }

        [Test]
        public void Generate_HasEqualityOperators() {
            var result = new ButtrIdentifierTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("operator =="));
            Assert.That(result, Does.Contain("operator !="));
        }
    }
}