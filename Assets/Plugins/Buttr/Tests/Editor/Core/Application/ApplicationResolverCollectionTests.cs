using System;
using Buttr.Core;
using NUnit.Framework;

// ReSharper disable ConvertTypeCheckToNullCheck
// ReSharper disable RedundantBoolCompare

namespace Buttr.Tests.Editor.Application {
    public sealed class ApplicationResolverCollectionTests{
        private interface IAbstract : IDisposable {
            bool Disposed { get; }
        }
        private sealed class Concrete : IAbstract {
            private bool m_IsDisposed;

            public bool Disposed => m_IsDisposed;
            
            public void Dispose() {
                m_IsDisposed = true;
            }
        }
        private sealed class DependantOnConcrete {
            private readonly Concrete m_Concrete;
            
            public DependantOnConcrete(Concrete concrete) {
                m_Concrete = concrete;
            }

            public Concrete Concrete => m_Concrete;
        }
        
        private IResolverCollection m_Collection = new ApplicationResolverCollection();
        
        [SetUp]
        public void SetUp() {
            m_Collection = new ApplicationResolverCollection();
        }

        [TearDown]
        public void TearDown() {
            m_Collection?.Dispose();
        }
        
        [Test] public void AddSingletonConcrete_ReturnsConcreteIConfigurable() {
            Assert.IsTrue(m_Collection.AddSingleton<Concrete>() is IConfigurable<Concrete>);
        }

        [Test] public void AddSingletonAbstractConcrete_ReturnsConcreteIConfigurable() { 
            Assert.IsTrue((m_Collection.AddSingleton<IAbstract, Concrete>()) is IConfigurable<Concrete>);
        }

        [Test] public void AddTransientConcrete_ReturnsConcreteIConfigurable() { 
            Assert.IsTrue(m_Collection.AddTransient<Concrete>() is IConfigurable<Concrete>);
        }

        [Test] public void AddTransientAbstractConcrrete_ReturnsConcreteIConfigurable() { 
            Assert.IsTrue(m_Collection.AddTransient<IAbstract, Concrete>() is IConfigurable<Concrete>);
        }

        [Test] public void Resolve_DoesNotThrow() { 
            Assert.DoesNotThrow(() => m_Collection.Resolve());
        }

        [Test] public void Resolve_DoesNotThrowIfMissingDependencies() {
            m_Collection.AddSingleton<DependantOnConcrete>();
            Assert.DoesNotThrow(() => m_Collection.Resolve());
        }

        [Test] public void Resolve_AddsServiceToStaticApplicationContainer() {
            m_Collection.AddSingleton<Concrete>();
            m_Collection.Resolve();
            Assert.DoesNotThrow(() => Application<Concrete>.Get());
        }

        [Test] public void Dispose_DisposesOfSingletonsIfResolved() { 
            m_Collection.AddSingleton<Concrete>();
            m_Collection.Resolve();

            var concrete = Application<Concrete>.Get();
            Assert.IsTrue(concrete.Disposed == false);
            
            m_Collection.Dispose();

            Assert.IsTrue(concrete.Disposed == true);
        }

        [Test] public void Dipose_RemovesServiceFromStaticApplicationContainer() {
            m_Collection.AddSingleton<Concrete>();
            m_Collection.Resolve();
            m_Collection.Dispose();
            
            Assert.Throws<NullReferenceException>( () => Application<Concrete>.Get());
        }
    }
}