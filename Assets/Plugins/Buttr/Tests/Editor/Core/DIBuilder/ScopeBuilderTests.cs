using Buttr.Core;
using NUnit.Framework;

// ReSharper disable ConvertTypeCheckToNullCheck
// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable RedundantTypeArgumentsOfMethod
// ReSharper disable RedundantBoolCompare

namespace Buttr.Tests.Editor.DIBuilder {
    public sealed class ScopeBuilderTests {
        interface IService0 { }
        class Service0 : IService0 { }
        
        private IDIBuilder m_Builder;
        
        [SetUp]
        public void Setup() {
            ScopeRegistry.RemoveScope("foo");
            m_Builder = new ScopeBuilder("Foo");
        }
        
        [Test]
        public void ScopeBuilder_AddTransient_TConcrete_ReturnsConfigurableOfTypeTConcrete() {
            var configurable = m_Builder.AddTransient<Service0>();
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void ScopeBuilder_AddTransient_TAbstract_TConcrete_ReturnsConfigurableOfTypeTConcrete() { 
            var configurable = m_Builder.AddTransient<IService0, Service0>();
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void ScopeBuilder_AddSingleton_TConcrete_ReturnsConfigurableOfTypeTConcrete() { 
            var configurable = m_Builder.AddSingleton<Service0>();
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void ScopeBuilder_AddSingleton_TAbstract_TConcrete_ReturnsConfigurableOfTypeTConcrete() { 
            var configurable = m_Builder.AddSingleton<IService0, Service0>();
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void ScopeBuilder_AddTheSameSingleton_TAbstract_TConcrete_TwiceDoesNotThrow() {
            m_Builder.AddSingleton<IService0, Service0>();
            
            // Can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddSingleton<IService0, Service0>());
        }
        
        [Test]
        public void ScopeBuilder_AddTheSameSingleton_TConcrete_TwiceThrows() {
            m_Builder.AddSingleton<Service0>();
            
            // Can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddSingleton<Service0>());
        }
        
        [Test]
        public void ScopeBuilder_AddTheSameTransient_TAbstract_TConcrete_TwiceThrows() {
            m_Builder.AddTransient<IService0, Service0>();
            
            // Can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddTransient<IService0, Service0>());
        }
        
        [Test]
        public void ScopeBuilder_AddTheSameTransient_TConcrete_TwiceThrows() {
            m_Builder.AddTransient<Service0>();
            
            // Can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddTransient<Service0>());
        }
        
        [Test]
        public void ScopeBuilder_AddTheSameTransientAs_TConcreteAndTAbstract_DoesNotThrow() {
            m_Builder.AddTransient<Service0>();
            
            // Can add the same resolver twice, if abstract and concrete
            Assert.DoesNotThrow(() => m_Builder.AddTransient<IService0, Service0>());
        }
        
        [Test]
        public void ScopeBuilder_AddTheSameSingletonAs_TConcreteAndTAbstract_DoesNotThrow() {
            m_Builder.AddSingleton<Service0>();
            
            // Can add the same resolver twice, if abstract and concrete
            Assert.DoesNotThrow(() => m_Builder.AddSingleton<IService0, Service0>());
        }

        [Test]
        public void ScopeBuilder_Build_DoesNotThrow() {
            ScopeRegistry.RemoveScope("foo");
            m_Builder.AddSingleton<IService0, Service0>();

            Assert.DoesNotThrow(() => {
                var container = m_Builder.Build(); container.Dispose();
            }, "Builder should not throw with 0 or more services provided");
            ScopeRegistry.RemoveScope("foo");
        }

        [Test]
        public void ScopeBuilder_Builder_ReturnsIDIContainer() { 
            ScopeRegistry.RemoveScope("foo");
            m_Builder.AddSingleton<IService0, Service0>();
            using var container = m_Builder.Build();
            
            Assert.IsTrue(container is IDIContainer);
        }
    }
}