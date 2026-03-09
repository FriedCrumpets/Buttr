using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrSystemTemplateTests {
        [Test]
        public void Generate_HasTickMethod() {
            var result = new ButtrSystemTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public void Tick(CombatContext ctx)"));
        }

        [Test]
        public void Generate_HasActiveBehaviourProperty() {
            var result = new ButtrSystemTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public ICombatBehaviour ActiveBehaviour"));
        }

        [Test]
        public void Generate_IsSealed() {
            var result = new ButtrSystemTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public sealed class CombatSystem"));
        }
    }
}