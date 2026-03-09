using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrConfigurationTemplateTests {
        [Test]
        public void Generate_IsSealed() {
            var result = new ButtrConfigurationTemplate("MyGame", "MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public sealed class CombatConfiguration"));
        }

        [Test]
        public void Generate_UsesProjectNameInMenuPath() {
            var result = new ButtrConfigurationTemplate("MyGame", "MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("menuName = \"MyGame/Configurations/Combat\""));
        }
    }
}