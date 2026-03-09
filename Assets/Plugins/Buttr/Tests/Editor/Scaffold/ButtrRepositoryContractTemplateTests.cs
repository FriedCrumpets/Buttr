using Buttr.Editor.Scaffolding;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrRepositoryContractTemplateTests {
        [Test]
        public void Generate_IsGenericInterface() {
            var result = new ButtrRepositoryContractTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("public interface IInventoryRepository<in TKey, TData>"));
        }

        [Test]
        public void Generate_HasCrudMethods() {
            var result = new ButtrRepositoryContractTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("void Create(TData entity)"));
            Assert.That(result, Does.Contain("TData Read(TKey id)"));
            Assert.That(result, Does.Contain("void Update(TData entity)"));
            Assert.That(result, Does.Contain("bool Delete(TData entity)"));
            Assert.That(result, Does.Contain("bool Delete(TKey id)"));
        }

        [Test]
        public void Generate_HasQueryMethods() {
            var result = new ButtrRepositoryContractTemplate("MyGame.Features.Inventory", "Inventory").Generate();
            Assert.That(result, Does.Contain("RetrieveAll()"));
            Assert.That(result, Does.Contain("RetrieveByCondition("));
            Assert.That(result, Does.Contain("Clear()"));
        }
    }
}