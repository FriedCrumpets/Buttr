using System;
using Buttr.Core;
using NUnit.Framework;

namespace Buttr.Tests.Editor.DIContainer {
    public sealed class ScopeContainerTests {
        private interface IService0 { }
        private sealed class Service0 : IService0 { }
        private interface IService1 : IHidden { int Test { get; } } 
        private sealed class Service1 : IService1 { public int Test { get; set; } = 1; }
        private interface IService2 { bool Validate(); }
        
        private sealed class Service2_1 : IService2 {
            private readonly IService0 m_Service0;
            private readonly Service1 m_Service1;
            
            public Service2_1(IService0 service0, Service1 service1) {
                m_Service0 = service0;
                m_Service1 = service1;
            }
            public bool Validate() {
                return m_Service0 is not null
                       && m_Service1 is not null;
            }
        }
        
        private sealed class Service2_2 : IService2 {
            private readonly IService1 m_Service1;
            
            public Service2_2(IService1 service1) {
                m_Service1 = service1;
            }
            
            public bool Validate() {
                return m_Service1 is { Test: 2 };
            }
        }
        
        private interface IService3 : IDisposable { bool Disposed { get; } }
        private sealed class Service3 : IService3 {
            public bool Disposed { get; private set; }

            public void Dispose() {
                Disposed = true;
            }
        }
        
        private IDIBuilder m_DIBuilder;

        [SetUp]
        public void Setup() {
            ScopeRegistry.RemoveScope("foo");
            m_DIBuilder = new ScopeBuilder("foo");
        }
        
        [Test] public void GetAbstractService_WhenNotRegistered_ReturnsDefault() {
            using var container = m_DIBuilder.Build();
            var service = container.Get<IService0>();
            
            // if not registered the service should be null
            Assert.IsTrue(service is null);
        }
        
        [Test] public void GetAbstractService_WhenRegistered_ReturnsService() {
            m_DIBuilder.AddSingleton<IService0, Service0>();
            using var container = m_DIBuilder.Build();
            var service = container.Get<IService0>();
            
            // if registered the service should not be null
            Assert.IsTrue(service is not null);
        }
        
        [Test] public void GetConcreteService_WhenNotRegistered_ReturnsDefault() {
            using var container = m_DIBuilder.Build();
            var service = container.Get<Service0>();
            
            // if not registered the service should be null
            Assert.IsTrue(service is null);
        }
        
        [Test] public void GetConcreteService_WhenRegistered_ReturnsService() { 
            m_DIBuilder.AddSingleton<Service0>();
            using var container = m_DIBuilder.Build();
            var service = container.Get<Service0>();
            
            // if registered the service should not be null
            Assert.IsTrue(service is not null);
        }
        
        [Test] public void GetConcreteService_WhenAbstractIsRegistered_ReturnsNull() { 
            m_DIBuilder.AddSingleton<IService0, Service0>();
            using var container = m_DIBuilder.Build();
            var service = container.Get<Service0>();
            
            // if not registered the service should be null
            Assert.IsTrue(service is null);
        }
        
        [Test] public void GetAbstractService_WhenConcreteIsRegistered_ReturnsNull() { 
            m_DIBuilder.AddSingleton<Service0>();
            using var container = m_DIBuilder.Build();
            var service = container.Get<IService0>();
            
            // if not registered the service should be null
            Assert.IsTrue(service is null);
        }
        
        [Test] public void TryGetConcreteService_WhenNotRegistered_ReturnsFalseAndValueIsDefault() { 
            using var container = m_DIBuilder.Build();
            var located = container.TryGet<Service0>(out var service);
            
            // if not registered the service should be null
            Assert.IsTrue(located is false && service is null);
        }
        
        [Test] public void TryGetConcreteService_WhenRegistered_ReturnsTrueAndValueIsService() {
            m_DIBuilder.AddSingleton<Service0>();
            using var container = m_DIBuilder.Build();
            var located = container.TryGet<Service0>(out var service);
            
            // if registered located should be true and the service should not be null
            Assert.IsTrue(located is true && service is Service0);
        }
        
        [Test] public void TryGetAbstractService_WhenNotRegistered_ReturnsFalseAndValueIsDefault() { 
            using var container = m_DIBuilder.Build();
            var located = container.TryGet<IService0>(out var service);
            
            // if not registered the service should be null
            Assert.IsTrue(located is false && service is null);
        }
        
        [Test] public void TryGetAbstractService_WhenRegistered_ReturnsTrueAndValueIsService() { 
            m_DIBuilder.AddSingleton<IService0, Service0>();
            using var container = m_DIBuilder.Build();
            var located = container.TryGet<IService0>(out var service);
            
            // if registered located should be true and the service should not be null
            Assert.IsTrue(located is true && service is IService0);
        }
        
        [Test] public void GetConcreteHiddenService_WhenNotRegistered_Throws() { 
            using var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.Get<Service1>());
        }
        
        [Test] public void GetConcreteHiddenService_WhenRegistered_Throws() { 
            m_DIBuilder.AddSingleton<Service1>();
            using var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.Get<Service1>());
        }
        
        [Test] public void GetAbstractHiddenService_WhenNotRegistered_Throws() { 
            using var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.Get<IService1>());
        }
        
        [Test] public void GetAbstractHiddenService_WhenRegistered_Throws() { 
            m_DIBuilder.AddSingleton<IService1, Service1>();
            using var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.Get<IService1>());
        }
        
        [Test] public void TryGetHiddenService_WhenNotRegistered_Throws() { 
            using var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.TryGet<IService1>(out var service));
        }
        
        [Test] public void TryGetHiddenService_WhenRegistered_Throws() { 
            m_DIBuilder.AddSingleton<IService1, Service1>();
            using var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.TryGet<IService1>(out var service));
        }
        
        [Test] public void GetService_WithRegisteredDependencies_ReturnsServiceWithDependenciesNotNull() {
            m_DIBuilder.AddSingleton<IService0, Service0>();
            m_DIBuilder.AddSingleton<Service1>();
            m_DIBuilder.AddSingleton<IService2, Service2_1>();
            using var container = m_DIBuilder.Build();
            
            // Validate all dependencies are located within the service
            Assert.IsTrue(container.Get<IService2>().Validate());
        }
        
        [Test] public void GetService_WithoutRegisteredDependencies_Throws() { 
            m_DIBuilder.AddSingleton<IService2, Service2_1>();
            using var container = m_DIBuilder.Build();
            
            // throws if dependencies are not located within the container
            Assert.Throws<ObjectResolverException>(() => container.Get<IService2>());
        }
        
        [Test] public void NotConfiguringConfigurationOfRegisteredService_CorrectlyInjectsDefaultService() {
            m_DIBuilder.AddSingleton<IService1, Service1>();
            m_DIBuilder.AddSingleton<IService2, Service2_2>();
            using var container = m_DIBuilder.Build();
            
            // Service2_2 expects int value to be 2, by default it is 1. 
            Assert.IsTrue(container.Get<IService2>().Validate() == false);
        }
        
        [Test] public void NotConfiguringFactoryOfRegisteredService_CorrectlyInjectsDefaultService() { 
            m_DIBuilder.AddSingleton<IService1, Service1>();
            m_DIBuilder.AddSingleton<IService2, Service2_2>();
            using var container = m_DIBuilder.Build();
            
            // Service2_2 expects int value to be 2, by default it is 1. 
            Assert.IsTrue(container.Get<IService2>().Validate() == false);
        }
        
        [Test] public void ConfiguringConfigurationOfRegisteredService_CorrectlyInjectsServiceWithConfiguration() { 
            m_DIBuilder.AddSingleton<IService1, Service1>().WithConfiguration(service => { service.Test = 2; return service; } );
            m_DIBuilder.AddSingleton<IService2, Service2_2>();
            using var container = m_DIBuilder.Build();
            
            // Service2_2 expects int value to be 2, by default it is 1, here it has been configured to 2. 
            Assert.IsTrue(container.Get<IService2>().Validate());
        }
        
        [Test] public void ConfiguringFactoryOfRegisteredService_CorrectlyInjectsServiceWithFactory() { 
            m_DIBuilder.AddSingleton<IService1, Service1>().WithFactory(() => { return new Service1 { Test = 2 }; } );
            m_DIBuilder.AddSingleton<IService2, Service2_2>();
            using var container = m_DIBuilder.Build();
            
            // Service2_2 expects int value to be 2, by default it is 1, here it has been created with a value of 2. 
            Assert.IsTrue(container.Get<IService2>().Validate());
        }
        
        [Test] public void DisposingOfContainer_DisposesOfDisposableRegisteredServices() {
            m_DIBuilder.AddSingleton<IService3, Service3>();
            var container = m_DIBuilder.Build();
            var service = container.Get<IService3>();
            Assert.IsTrue(service.Disposed == false);
            container.Dispose();
            Assert.IsTrue(service.Disposed == true);
        }

        [Test] public void AddingScopeOfSameNameTwice_Throws() {
            m_DIBuilder.Build();
            Assert.Throws<ArgumentException>(() => m_DIBuilder.Build());
        }
    }
}