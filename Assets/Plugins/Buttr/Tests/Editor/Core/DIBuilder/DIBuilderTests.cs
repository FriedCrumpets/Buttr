using Buttr.Core;
using NUnit.Framework;

// ReSharper disable RedundantDefaultMemberInitializer
// ReSharper disable RedundantTypeArgumentsOfMethod
// ReSharper disable RedundantBoolCompare

namespace Buttr.Tests.Editor.DIBuilder {
    public sealed class DIBuilderTests {
        interface IService0 { }
        class Service0 : IService0 { }
        
        private IDIBuilder m_Builder;
        
        [SetUp]
        public void Setup() {
            m_Builder = new Core.DIBuilder();
        }
        
        [Test]
        public void DIBuilder_AddTransient_TConcrete_ReturnsConfigurableOfTypeTConcrete() {
            var configurable = m_Builder.AddTransient<Service0>();
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void DIBuilder_AddTransient_TAbstract_TConcrete_ReturnsConfigurableOfTypeTConcrete() { 
            var configurable = m_Builder.AddTransient<IService0, Service0>();
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void DIBuilder_AddSingleton_TConcrete_ReturnsConfigurableOfTypeTConcrete() { 
            var configurable = m_Builder.AddSingleton<Service0>();
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void DIBuilder_AddSingleton_TAbstract_TConcrete_ReturnsConfigurableOfTypeTConcrete() { 
            var configurable = m_Builder.AddSingleton<IService0, Service0>();
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void DIBuilder_AddTheSameSingleton_TAbstract_TConcrete_TwiceDoesNotThrow() {
            m_Builder.AddSingleton<IService0, Service0>();
            
            // Can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddSingleton<IService0, Service0>());
        }
        
        [Test]
        public void DIBuilder_AddTheSameSingleton_TConcrete_TwiceThrows() {
            m_Builder.AddSingleton<Service0>();
            
            // Can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddSingleton<Service0>());
        }
        
        [Test]
        public void DIBuilder_AddTheSameTransient_TAbstract_TConcrete_TwiceThrows() {
            m_Builder.AddTransient<IService0, Service0>();
            
            // Can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddTransient<IService0, Service0>());
        }
        
        [Test]
        public void DIBuilder_AddTheSameTransient_TConcrete_TwiceThrows() {
            m_Builder.AddTransient<Service0>();
            
            // Can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddTransient<Service0>());
        }
        
        [Test]
        public void DIBuilder_AddTheSameTransientAs_TConcreteAndTAbstract_DoesNotThrow() {
            m_Builder.AddTransient<Service0>();
            
            // Can add the same resolver twice, if abstract and concrete
            Assert.DoesNotThrow(() => m_Builder.AddTransient<IService0, Service0>());
        }
        
        [Test]
        public void DIBuilder_AddTheSameSingletonAs_TConcreteAndTAbstract_DoesNotThrow() {
            m_Builder.AddSingleton<Service0>();
            
            // Can add the same resolver twice, if abstract and concrete
            Assert.DoesNotThrow(() => m_Builder.AddSingleton<IService0, Service0>());
        }

        [Test]
        public void DIBuilder_Build_DoesNotThrow() {
            m_Builder.AddSingleton<IService0, Service0>();

            Assert.DoesNotThrow(() => m_Builder.Build(), "Builder should not throw with 0 or more services provided");
        }

        [Test]
        public void DIBuilder_Builder_ReturnsIDIContainer() { 
            m_Builder.AddSingleton<IService0, Service0>();
            var container = m_Builder.Build();
            
            Assert.IsTrue(container is IDIContainer);
        }
    }
}