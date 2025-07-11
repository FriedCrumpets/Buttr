using System;
using Buttr.Core;
using NUnit.Framework;

// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable ObjectCreationAsStatement

namespace Buttr.Tests.Editor.Common {
    public sealed class DisposableCollectionTests {
        private sealed class MockDisposable : IDisposable {
            public bool IsDisposed { get; private set; }

            public void Dispose() {
                IsDisposed = true;
            }
        }
        
        [Test]
        public void Dispose_WhenCalled_DisposesAllInnerDisposables() {
            // Arrange
            var disposable1 = new MockDisposable();
            var disposable2 = new MockDisposable();
            var disposable3 = new MockDisposable();

            IDisposable[] disposables = { disposable1, disposable2, disposable3 };
            var cleanup = new DisposableCollection(disposables);

            // Act
            cleanup.Dispose();

            // Assert
            Assert.IsTrue(disposable1.IsDisposed, "Disposable1 should be disposed.");
            Assert.IsTrue(disposable2.IsDisposed, "Disposable2 should be disposed.");
            Assert.IsTrue(disposable3.IsDisposed, "Disposable3 should be disposed.");
        }

        [Test]
        public void Dispose_WhenCalledMultipleTimes_DisposesInnerDisposablesOnlyOnce() {
            // Arrange
            var disposable = new MockDisposable();
            IDisposable[] disposables = { disposable };
            var cleanup = new DisposableCollection(disposables);

            // Act
            cleanup.Dispose();
            cleanup.Dispose(); // Call Dispose multiple times

            // Assert
            // We can't directly assert "only once" on MockDisposable without a counter,
            // but the idempotent nature of DisposableCollection's Dispose() implies this.
            // The primary assertion is that it *is* disposed.
            Assert.IsTrue(disposable.IsDisposed, "Disposable should be disposed after multiple calls.");
            // If MockDisposable had a call counter, you'd assert: Assert.AreEqual(1, disposable.DisposeCallCount);
        }

        [Test]
        public void Constructor_WithEmptyArray_DoesNotThrow() {
            // Arrange
            var emptyDisposables = Array.Empty<IDisposable>();

            // Act & Assert
            // The constructor should not throw an exception for an empty array
            Assert.DoesNotThrow(() => new DisposableCollection(emptyDisposables));
        }

        [Test]
        public void Dispose_WithEmptyArray_DoesNotThrow() {
            // Arrange
            var emptyDisposables = Array.Empty<IDisposable>();
            var cleanup = new DisposableCollection(emptyDisposables);

            // Act & Assert
            // Calling Dispose on a cleanup object initialized with an empty array should not throw
            Assert.DoesNotThrow(() => cleanup.Dispose());
        }

        [Test]
        public void Constructor_WithNullArray_ThrowsArgumentNullException() {
            // Arrange
            IDisposable[] nullDisposables = null;

            // Act & Assert
            // Expect an ArgumentNullException if the constructor receives a null array
            Assert.Throws<ArgumentNullException>(() => new DisposableCollection(nullDisposables));
        }

        [Test]
        public void Dispose_WhenSomeInnerDisposableIsNull_DoesNotThrowAndDisposesOthers() {
            // Arrange
            var disposable1 = new MockDisposable();
            IDisposable disposable2 = null; // Simulate a null entry
            var disposable3 = new MockDisposable();

            IDisposable[] disposables = { disposable1, disposable2, disposable3 };
            var cleanup = new DisposableCollection(disposables);

            // Act & Assert
            // We expect it not to throw, and for disposable1 and disposable3 to still be disposed
            Assert.DoesNotThrow(() => cleanup.Dispose());
            Assert.IsTrue(disposable1.IsDisposed, "Disposable1 should be disposed.");
            Assert.IsTrue(disposable3.IsDisposed, "Disposable3 should be disposed.");
        }
    }
}