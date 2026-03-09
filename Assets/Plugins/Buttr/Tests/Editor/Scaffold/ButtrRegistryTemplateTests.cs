using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrRegistryTemplateTests {
        [Test]
        public void Generate_UsesIdAndControllerTypes() {
            var result = new ButtrRegistryTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("Dictionary<CombatId, CombatController>"));
        }

        [Test]
        public void Generate_RegisterReturnsIDisposable() {
            var result = new ButtrRegistryTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public IDisposable Register(CombatId id, CombatController entry)"));
        }

        [Test]
        public void Generate_HasTryGetMethod() {
            var result = new ButtrRegistryTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public bool TryGet(CombatId id, out CombatController entry)"));
        }

        [Test]
        public void Generate_HasGetMethod() {
            var result = new ButtrRegistryTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public CombatController Get(CombatId id)"));
        }

        [Test]
        public void Generate_DeregisterIsPrivate() {
            var result = new ButtrRegistryTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("private void Deregister(CombatId id)"));
        }

        [Test]
        public void Generate_HasNestedRegistrationClass() {
            var result = new ButtrRegistryTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("private sealed class Registration : IDisposable"));
        }

        [Test]
        public void Generate_ExposesValuesAndCount() {
            var result = new ButtrRegistryTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public IReadOnlyCollection<CombatController> Values"));
            Assert.That(result, Does.Contain("public int Count"));
        }
    }
}