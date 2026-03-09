using System;
using System.IO;
using Buttr.Editor.Scaffolding;
using Buttr.Editor.SetupWizard;
using NUnit.Framework;

namespace Buttr.Editor.Tests.Scaffolding {
    [TestFixture]
    public sealed class ButtrPackageScaffolderUtilityTests {
        private string m_TestRoot;

        [SetUp]
        public void SetUp() {
            m_TestRoot = Path.Combine(Path.GetTempPath(), "ButtrTests_" + Guid.NewGuid().ToString("N")[..8]);
            Directory.CreateDirectory(m_TestRoot);
        }

        [TearDown]
        public void TearDown() {
            if (Directory.Exists(m_TestRoot))
                Directory.Delete(m_TestRoot, true);
        }

        [Test]
        public void CreateSubFolder_CreatesDirectory() {
            var result = m_TestRoot.CreateSubFolder("Components");
            Assert.That(Directory.Exists(result), Is.True);
        }

        [Test]
        public void CreateSubFolder_ReturnsFullPath() {
            var result = m_TestRoot.CreateSubFolder("Components");
            Assert.That(result, Is.EqualTo(Path.Combine(m_TestRoot, "Components")));
        }

        [Test]
        public void EnsureSubFolder_CreatesIfNotExists() {
            var result = m_TestRoot.EnsureSubFolder("Contracts");
            Assert.That(Directory.Exists(result), Is.True);
        }

        [Test]
        public void EnsureSubFolder_DoesNotThrowIfExists() {
            Directory.CreateDirectory(Path.Combine(m_TestRoot, "Contracts"));
            Assert.DoesNotThrow(() => m_TestRoot.EnsureSubFolder("Contracts"));
        }

        [Test]
        public void WriteFile_CreatesFile() {
            m_TestRoot.WriteFile("Test.cs", "content");
            Assert.That(File.Exists(Path.Combine(m_TestRoot, "Test.cs")), Is.True);
        }

        [Test]
        public void WriteFile_WritesContent() {
            m_TestRoot.WriteFile("Test.cs", "hello world");
            var content = File.ReadAllText(Path.Combine(m_TestRoot, "Test.cs"));
            Assert.That(content, Is.EqualTo("hello world"));
        }

        [Test]
        public void WriteFileIfNew_CreatesFile() {
            m_TestRoot.WriteFileIfNew("Test.cs", "content");
            Assert.That(File.Exists(Path.Combine(m_TestRoot, "Test.cs")), Is.True);
        }

        [Test]
        public void WriteFileIfNew_DoesNotOverwriteExisting() {
            var path = Path.Combine(m_TestRoot, "Test.cs");
            File.WriteAllText(path, "original");
            m_TestRoot.WriteFileIfNew("Test.cs", "replacement");
            var content = File.ReadAllText(path);
            Assert.That(content, Is.EqualTo("original"));
        }

        [Test]
        public void SanitiseNamespace_RemovesSpaces() {
            var result = ButtrProjectScaffolder.SanitiseNamespace("My Game");
            Assert.That(result, Is.EqualTo("MyGame"));
        }

        [Test]
        public void SanitiseNamespace_RemovesSpecialCharacters() {
            var result = ButtrProjectScaffolder.SanitiseNamespace("My-Game!2024");
            Assert.That(result, Is.EqualTo("MyGame2024"));
        }

        [Test]
        public void SanitiseNamespace_ReturnsProject_WhenEmpty() {
            var result = ButtrProjectScaffolder.SanitiseNamespace("");
            Assert.That(result, Is.EqualTo("Project"));
        }

        [Test]
        public void SanitiseNamespace_ReturnsProject_WhenWhitespace() {
            var result = ButtrProjectScaffolder.SanitiseNamespace("   ");
            Assert.That(result, Is.EqualTo("Project"));
        }

        [Test]
        public void SanitiseNamespace_ReturnsProject_WhenAllSpecialChars() {
            var result = ButtrProjectScaffolder.SanitiseNamespace("!@#$%");
            Assert.That(result, Is.EqualTo("Project"));
        }

        [Test]
        public void SanitiseNamespace_StripsLeadingDigits() {
            var result = ButtrProjectScaffolder.SanitiseNamespace("123Game");
            Assert.That(result, Is.EqualTo("Game"));
        }

        [Test]
        public void SanitiseNamespace_AllowsUnderscorePrefix() {
            var result = ButtrProjectScaffolder.SanitiseNamespace("_MyGame");
            Assert.That(result, Is.EqualTo("_MyGame"));
        }

        [Test]
        public void SanitiseNamespace_AllowsUnderscoresInMiddle() {
            var result = ButtrProjectScaffolder.SanitiseNamespace("My_Game_2024");
            Assert.That(result, Is.EqualTo("My_Game_2024"));
        }
    }
}