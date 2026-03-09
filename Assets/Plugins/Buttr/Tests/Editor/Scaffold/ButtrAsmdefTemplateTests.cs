using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrAsmdefTemplateTests {
        [Test]
        public void Generate_UsesNamespaceAsName() {
            var result = new ButtrAsmdefTemplate("MyGame.Features.Inventory").Generate();
            Assert.That(result, Does.Contain("\"name\": \"MyGame.Features.Inventory\""));
        }

        [Test]
        public void Generate_UsesNamespaceAsRootNamespace() {
            var result = new ButtrAsmdefTemplate("MyGame.Features.Inventory").Generate();
            Assert.That(result, Does.Contain("\"rootNamespace\": \"MyGame.Features.Inventory\""));
        }

        [Test]
        public void Generate_ReferencesButtrCore() {
            var result = new ButtrAsmdefTemplate("MyGame.Features.Inventory").Generate();
            Assert.That(result, Does.Contain("Buttr.Core"));
        }

        [Test]
        public void Generate_ReferencesButtrUnity() {
            var result = new ButtrAsmdefTemplate("MyGame.Features.Inventory").Generate();
            Assert.That(result, Does.Contain("Buttr.Unity"));
        }

        [Test]
        public void Generate_DoesNotReferenceProjectSpecificAssemblies() {
            var result = new ButtrAsmdefTemplate("MyGame.Features.Inventory").Generate();
            // Should only have Buttr.Core and Buttr.Unity, no project-specific references
            Assert.That(result, Does.Not.Contain("MyGame.Core"));
        }
    }
}