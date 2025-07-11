using System;
using Buttr.Core;
using NUnit.Framework;

namespace Buttr.Tests.Editor.Application {
    public sealed class ApplicationBuilderTests {
        private interface IAbstract { }
        private sealed class Concrete : IAbstract { }
        private sealed class Hidden { }
        private sealed class ConcreteReliesOnHidden : IAbstract {
            public ConcreteReliesOnHidden(Hidden hidden) { }
        }

        private ApplicationBuilder m_Builder;
        private ApplicationLifetime m_Lifetime;
        
        [SetUp]
        public void SetUp() {
            m_Builder = new ApplicationBuilder();
        }

        [TearDown]
        public void TearDown() {
            m_Lifetime?.Dispose();   
        }
        
        [Test] public void Build_DoesNotThrow() {
            Assert.DoesNotThrow(() => m_Builder.Build());
        }
        
        [Test] public void Build_ResolvesHiddenCollection() {
            m_Builder.Hidden.AddSingleton<Hidden>();
            m_Builder.Resolvers.AddSingleton<IAbstract, ConcreteReliesOnHidden>();
            m_Lifetime = m_Builder.Build();
            Assert.DoesNotThrow(() => Application<IAbstract>.Get());
        }
        
        [Test] public void Build_ResolvesResolverCollection() { 
            m_Builder.Resolvers.AddSingleton<IAbstract, Concrete>();
            m_Lifetime = m_Builder.Build();
            Assert.DoesNotThrow(() => Application<IAbstract>.Get());
        }

        private sealed class Disposable : IDisposable {
            public bool Disposed { get; private set; }
            
            public void Dispose() {
                Disposed = true;
            }
        }
        
        [Test] public void IfCleanupProvided_CleanupIsAddedToApplicationRunner() {
            var disposed = new Disposable();
            m_Builder.Cleanup = new DisposableCollection(disposed);
            m_Lifetime = m_Builder.Build();
            m_Lifetime.Dispose();
            
            Assert.IsTrue(disposed.Disposed == true);
        }
    }
}