using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrHandlerTemplateTests {
        [Test]
        public void Generate_IsAbstract() {
            var result = new ButtrHandlerTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public abstract class CombatHandler"));
        }

        [Test]
        public void Generate_ExtendsScriptableObject() {
            var result = new ButtrHandlerTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("ScriptableObject"));
        }
    }
}