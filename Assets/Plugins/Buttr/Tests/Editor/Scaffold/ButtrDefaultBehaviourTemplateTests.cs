using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrDefaultBehaviourTemplateTests {
        [Test]
        public void Generate_ImplementsInterface() {
            var result = new ButtrDefaultBehaviourTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("ICombatBehaviour"));
        }

        [Test]
        public void Generate_IsSealed() {
            var result = new ButtrDefaultBehaviourTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public sealed class DefaultCombatBehaviour"));
        }

        [Test]
        public void Generate_HasTickMethod() {
            var result = new ButtrDefaultBehaviourTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public void Tick(CombatContext ctx)"));
        }
    }
}