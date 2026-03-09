using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrDefinitionTemplateTests {
        [Test]
        public void Generate_ExtendsScriptableObject() {
            var result = new ButtrDefinitionTemplate("MyGame", "MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("ScriptableObject"));
        }

        [Test]
        public void Generate_HasCreateAssetMenu() {
            var result = new ButtrDefinitionTemplate("MyGame", "MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("CreateAssetMenu"));
        }

        [Test]
        public void Generate_UsesProjectNameInMenuPath() {
            var result = new ButtrDefinitionTemplate("MyGame", "MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("menuName = \"MyGame/Definitions/Combat\""));
        }

        [Test]
        public void Generate_IsNotSealed() {
            var result = new ButtrDefinitionTemplate("MyGame", "MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public class CombatDefinition"));
            Assert.That(result, Does.Not.Contain("sealed"));
        }
    }
}