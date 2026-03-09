using System;
using System.IO;
using NUnit.Framework;
using Buttr.Editor.Scaffolding;

namespace Buttr.Editor.Tests.Scaffolding {
    public sealed class AddCommandFileSystemTests {
        private string m_TestRoot;
        private string m_PackageRoot;

        [SetUp]
        public void SetUp() {
            m_TestRoot = Path.Combine(Path.GetTempPath(), "ButtrCmdTests_" + Guid.NewGuid().ToString("N")[..8]);
            m_PackageRoot = Path.Combine(m_TestRoot, "_Project", "Features", "Inventory");
            Directory.CreateDirectory(m_PackageRoot);

            // Create a fake asmdef so InferPackage can find the package root
            File.WriteAllText(
                Path.Combine(m_PackageRoot, "TestGame.Features.Inventory.asmdef"),
                "{}"
            );
        }

        [TearDown]
        public void TearDown() {
            if (Directory.Exists(m_TestRoot))
                Directory.Delete(m_TestRoot, true);
        }

        [Test]
        public void AddModelCommand_CreatesComponentsFolder() {
            new AddModelCommand(m_PackageRoot, false).Execute();
            Assert.That(Directory.Exists(Path.Combine(m_PackageRoot, "Components")), Is.True);
        }

        [Test]
        public void AddModelCommand_CreatesModelFile() {
            new AddModelCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Components", "InventoryModel.cs")), Is.True);
        }

        [Test]
        public void AddPresenterCommand_CreatesPresenterFile() {
            new AddPresenterCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Components", "InventoryPresenter.cs")), Is.True);
        }

        [Test]
        public void AddMediatorCommand_CreatesMediatorFile() {
            new AddMediatorCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Components", "InventoryMediator.cs")), Is.True);
        }

        [Test]
        public void AddServiceAndContractCommand_CreatesServiceFile() {
            new AddServiceAndContractCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Components", "InventoryService.cs")), Is.True);
        }

        [Test]
        public void AddServiceAndContractCommand_CreatesContractFile() {
            new AddServiceAndContractCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Contracts", "IInventoryService.cs")), Is.True);
        }

        [Test]
        public void AddViewCommand_CreatesMonoBehavioursFolder() {
            new AddViewCommand(m_PackageRoot, false).Execute();
            Assert.That(Directory.Exists(Path.Combine(m_PackageRoot, "MonoBehaviours")), Is.True);
        }

        [Test]
        public void AddViewCommand_CreatesViewFile() {
            new AddViewCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "MonoBehaviours", "InventoryView.cs")), Is.True);
        }

        [Test]
        public void AddControllerCommand_CreatesControllerFile() {
            new AddControllerCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "MonoBehaviours", "InventoryController.cs")), Is.True);
        }

        [Test]
        public void AddSystemCommand_CreatesSystemFile() {
            new AddSystemCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Components", "InventorySystem.cs")), Is.True);
        }

        [Test]
        public void AddSystemCommand_CreatesContextFile() {
            new AddSystemCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Common", "InventoryContext.cs")), Is.True);
        }

        [Test]
        public void AddBehaviourCommand_CreatesInterfaceFile() {
            new AddBehaviourCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Contracts", "IInventoryBehaviour.cs")), Is.True);
        }

        [Test]
        public void AddBehaviourCommand_CreatesDefaultBehaviourFile() {
            new AddBehaviourCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Behaviours", "DefaultInventoryBehaviour.cs")), Is.True);
        }

        [Test]
        public void AddBehaviourCommand_CreatesContextFile() {
            new AddBehaviourCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Common", "InventoryContext.cs")), Is.True);
        }

        [Test]
        public void AddIdentifierCommand_CreatesIdentifierFile() {
            new AddIdentifierCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Identifiers", "InventoryId.cs")), Is.True);
        }

        [Test]
        public void AddHandlerCommand_CreatesHandlerFile() {
            new AddHandlerCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Handlers", "InventoryHandler.cs")), Is.True);
        }

        [Test]
        public void AddRepositoryCommand_CreatesContractFile() {
            new AddRepositoryCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Contracts", "IInventoryRepository.cs")), Is.True);
        }

        [Test]
        public void AddRegistryCommand_CreatesRegistryFile() {
            new AddRegistryCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Components", "InventoryRegistry.cs")), Is.True);
        }

        [Test]
        public void AddRegistryCommand_CreatesIdentifierDependency() {
            new AddRegistryCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Identifiers", "InventoryId.cs")), Is.True);
        }

        [Test]
        public void AddRegistryCommand_CreatesControllerDependency() {
            new AddRegistryCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "MonoBehaviours", "InventoryController.cs")), Is.True);
        }

        [Test]
        public void AddExtensionsCommand_CreatesAtPackageRoot() {
            new AddExtensionsCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "InventoryExtensions.cs")), Is.True);
        }

        [Test]
        public void AddExceptionCommand_CreatesExceptionFile() {
            new AddExceptionCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Exceptions", "InventoryException.cs")), Is.True);
        }

        [Test]
        public void AddModelCommand_DoesNotOverwriteExistingFile() {
            var componentsFolder = Path.Combine(m_PackageRoot, "Components");
            Directory.CreateDirectory(componentsFolder);
            var filePath = Path.Combine(componentsFolder, "InventoryModel.cs");
            File.WriteAllText(filePath, "// existing content");

            new AddModelCommand(m_PackageRoot, false).Execute();

            var content = File.ReadAllText(filePath);
            Assert.That(content, Is.EqualTo("// existing content"));
        }

        [Test]
        public void AddSystemCommand_DoesNotOverwriteExistingContext() {
            var commonFolder = Path.Combine(m_PackageRoot, "Common");
            Directory.CreateDirectory(commonFolder);
            var filePath = Path.Combine(commonFolder, "InventoryContext.cs");
            File.WriteAllText(filePath, "// existing context");

            new AddSystemCommand(m_PackageRoot, false).Execute();

            var content = File.ReadAllText(filePath);
            Assert.That(content, Is.EqualTo("// existing context"));
        }

        [Test]
        public void AddBehaviourCommand_DoesNotOverwriteExistingContext() {
            var commonFolder = Path.Combine(m_PackageRoot, "Common");
            Directory.CreateDirectory(commonFolder);
            var filePath = Path.Combine(commonFolder, "InventoryContext.cs");
            File.WriteAllText(filePath, "// existing context");

            new AddBehaviourCommand(m_PackageRoot, false).Execute();

            var content = File.ReadAllText(filePath);
            Assert.That(content, Is.EqualTo("// existing context"));
        }

        [Test]
        public void AddRegistryCommand_DoesNotOverwriteExistingIdentifier() {
            var idFolder = Path.Combine(m_PackageRoot, "Identifiers");
            Directory.CreateDirectory(idFolder);
            var filePath = Path.Combine(idFolder, "InventoryId.cs");
            File.WriteAllText(filePath, "// existing id");

            new AddRegistryCommand(m_PackageRoot, false).Execute();

            var content = File.ReadAllText(filePath);
            Assert.That(content, Is.EqualTo("// existing id"));
        }

        [Test]
        public void Commands_WorkFromSubFolder() {
            // Simulate right-clicking inside Components/
            var componentsFolder = Path.Combine(m_PackageRoot, "Components");
            Directory.CreateDirectory(componentsFolder);

            // InferPackage should walk up to the package root
            new AddHandlerCommand(m_PackageRoot, false).Execute();
            Assert.That(File.Exists(Path.Combine(m_PackageRoot, "Handlers", "InventoryHandler.cs")), Is.True);
        }
    }
}