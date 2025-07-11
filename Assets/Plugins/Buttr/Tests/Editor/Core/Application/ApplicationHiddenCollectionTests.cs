using System;
using Buttr.Core;
using NUnit.Framework;

// ReSharper disable ConvertTypeCheckToNullCheck
// ReSharper disable RedundantBoolCompare

namespace Buttr.Tests.Editor.Application {
    public sealed class ApplicationHiddenCollectionTests {
        private interface IAbstract : IDisposable {
            bool Disposed { get; }
        }
        private sealed class Concrete : IAbstract {
            private bool s_IsDisposed;

            public bool Disposed => s_IsDisposed;
            
            public void Dispose() {
                s_IsDisposed = true;
            }
        }
        private sealed class DependantOnConcrete {
            private readonly Concrete m_Concrete;
            
            public DependantOnConcrete(Concrete concrete) {
                m_Concrete = concrete;
            }

            public Concrete Concrete => m_Concrete;
        }
        
        private IResolverCollection m_Collection;
        
        [SetUp]
        public void SetUp() {
            m_Collection = new ApplicationHiddenCollection();
        }

        [TearDown]
        public void TearDown() {
            m_Collection?.Dispose();
        }
        
        [Test] public void AddSingletonConcrete_ReturnsConcreteIConfigurable() {
            Assert.IsTrue(m_Collection.AddSingleton<Concrete>() is IConfigurable<Concrete>);
        }

        [Test] public void AddSingletonAbstractConcrete_ReturnsConcreteIConfigurable() { 
            Assert.IsTrue(m_Collection.AddSingleton<IAbstract, Concrete>() is IConfigurable<Concrete>);
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
            using var collection2 = new ApplicationResolverCollection();
            collection2.AddSingleton<DependantOnConcrete>();
            m_Collection.Resolve();
            collection2.Resolve();
            Assert.DoesNotThrow(() => Application<DependantOnConcrete>.Get());
        }

        [Test] public void Dispose_DisposesOfSingletonsIfResolved() { 
            m_Collection.AddSingleton<Concrete>();
            
            using var collection2 = new ApplicationResolverCollection();
            collection2.AddSingleton<DependantOnConcrete>();
            
            m_Collection.Resolve();
            collection2.Resolve();
            
            var concrete = Application<DependantOnConcrete>.Get().Concrete;
            Assert.IsTrue(concrete.Disposed == false);
            
            m_Collection.Dispose();

            Assert.IsTrue(concrete.Disposed == true);
        }

        [Test] public void Dipose_RemovesHiddenServiceFromStaticApplicationContainer() {
            m_Collection.AddSingleton<Concrete>();
            
            using var collection2 = new ApplicationResolverCollection();
            collection2.AddSingleton<DependantOnConcrete>();
            
            m_Collection.Resolve();
            collection2.Resolve();
            
            m_Collection.Dispose();
            Assert.Throws<ObjectResolverException>( () => Application<DependantOnConcrete>.Get());
        }
    }
}