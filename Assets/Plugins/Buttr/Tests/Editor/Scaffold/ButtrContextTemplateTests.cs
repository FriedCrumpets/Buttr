using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrContextTemplateTests {
        [Test]
        public void Generate_IsReadonlyStruct() {
            var result = new ButtrContextTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public readonly struct CombatContext"));
        }

        [Test]
        public void Generate_HasDeltaTimeField() {
            var result = new ButtrContextTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("DeltaTime"));
        }

        [Test]
        public void Generate_HasConstructor() {
            var result = new ButtrContextTemplate("MyGame.Features.Combat", "Combat").Generate();
            Assert.That(result, Does.Contain("public CombatContext(float deltaTime)"));
        }
    }
}