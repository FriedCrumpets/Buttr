using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrBehaviourInterfaceTemplateTests {
        [Test]
        public void Generate_IsInterface() {
            var result = new ButtrBehaviourInterfaceTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public interface ICombatBehaviour"));
        }

        [Test]
        public void Generate_HasTickMethod() {
            var result = new ButtrBehaviourInterfaceTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("void Tick(CombatContext ctx)"));
        }
    }
}