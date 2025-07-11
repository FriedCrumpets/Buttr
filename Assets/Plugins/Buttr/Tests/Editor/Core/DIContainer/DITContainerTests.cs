using System;
using Buttr.Core;
using NUnit.Framework;

namespace Buttr.Tests.Editor.DIContainer {
    public sealed class DITContainerTests {
        private interface IService0 { }
        private sealed class Service0 : IService0 { }
        private interface IService1 : IHidden { int Test { get; } } 
        private sealed class Service1 : IService1 { public int Test { get; set; } = 1; }
        
        private interface IService2 { int Test { get; } } 
        private sealed class Service2 : IService2 {
            public int Test { get; set; }
        }
        
        private interface IService3 : IDisposable { bool Disposed { get; } }
        private sealed class Service3 : IService3 {
            public bool Disposed { get; private set; }

            public void Dispose() {
                Disposed = true;
            }
        }
        
        public readonly struct TID : IEquatable<TID> {
            private TID(int id) { ID = id; }

            public int ID { get; }

            public static implicit operator int(TID entity) => entity.ID;
            public static implicit operator TID(int id) => new(id);

            public override string ToString() {
                return ID.ToString();
            }
        
            public override int GetHashCode() {
                return (ID.GetHashCode());
            }

            public override bool Equals(object obj) {
                return obj is TID other && Equals(other);
            }
        
            public bool Equals(TID other) {
                return ID == other.ID;
            }

            public static bool operator ==(TID left, TID right) => left.Equals(right);
            public static bool operator !=(TID left, TID right) => !left.Equals(right);
        }
        
        private IDIBuilder<TID> m_DIBuilder;

        [SetUp]
        public void Setup() {
            m_DIBuilder = new DIBuilder<TID>();
        }
        
        [Test] public void GetConcreteService_WhenNotRegistered_ReturnsDefault() {
            var container = m_DIBuilder.Build();
            var service = container.Get<Service0>(0);
            
            // if not registered the service should be null
            Assert.IsTrue(service is null);
        }
        
        [Test] public void GetConcreteService_WhenRegistered_ReturnsService() { 
            m_DIBuilder.AddSingleton<Service0>(0);
            var container = m_DIBuilder.Build();
            var service = container.Get<Service0>(0);
            
            // if registered the service should not be null
            Assert.IsTrue(service is not null);
        }
        
        [Test] public void GetAbstractService_WhenConcreteIsRegistered_ReturnsService() { 
            m_DIBuilder.AddSingleton<Service0>(0);
            var container = m_DIBuilder.Build();
            var service = container.Get<IService0>(0);
            
            // if not registered the service should be null
            Assert.IsTrue(service is IService0);
        }
        
        [Test] public void GetIncorrectAbstractionForService_WhenConcreteIsRegistered_Throws() { 
            m_DIBuilder.AddSingleton<Service0>(0);
            var container = m_DIBuilder.Build();
            Assert.Throws<InvalidCastException>(() => container.Get<IService2>(0));
        }
        
        [Test] public void RegisterAbstractService_Throws() { 
            Assert.Throws<ArgumentException>(() => m_DIBuilder.AddSingleton<IService0>(0));
        }
        
        [Test] public void TryGetConcreteService_WhenNotRegistered_ReturnsFalseAndValueIsDefault() { 
            var container = m_DIBuilder.Build();
            var located = container.TryGet<Service0>(0, out var service);
            
            // if not registered the service should be null
            Assert.IsTrue(located is false && service is null);
        }
        
        [Test] public void TryGetConcreteService_WhenRegistered_ReturnsTrueAndValueIsService() {
            m_DIBuilder.AddSingleton<Service0>(0);
            var container = m_DIBuilder.Build();
            var located = container.TryGet<Service0>(0, out var service);
            
            // if registered located should be true and the service should not be null
            Assert.IsTrue(located is true && service is Service0);
        }
        
        [Test] public void GetConcreteHiddenService_WhenNotRegistered_Throws() { 
            var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.Get<Service1>(0));
        }
        
        [Test] public void GetConcreteHiddenService_WhenRegistered_Throws() { 
            m_DIBuilder.AddSingleton<Service1>(0);
            var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.Get<Service1>(0));
        }
        
        [Test] public void GetAbstractHiddenService_WhenNotRegistered_Throws() { 
            var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.Get<IService1>(0));
        }
        
        [Test] public void GetAbstractHiddenService_WhenRegistered_Throws() { 
            m_DIBuilder.AddSingleton<Service1>(0);
            var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.Get<IService1>(0));
        }
        
        [Test] public void TryGetHiddenService_WhenNotRegistered_Throws() { 
            var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.TryGet<Service1>(0, out var service));
        }
        
        [Test] public void TryGetHiddenService_WhenRegistered_Throws() { 
            m_DIBuilder.AddSingleton<Service1>(0);
            var container = m_DIBuilder.Build();
            
            // cannot get a hidden service
            Assert.Throws<ObjectResolverException>(() => container.TryGet<Service1>(0, out var service));
        }
        
        [Test] public void NotConfiguringConfigurationOfRegisteredService_CorrectlyInjectsDefaultService() {
            m_DIBuilder.AddSingleton<Service2>(0);
            var container = m_DIBuilder.Build();
            
            Assert.IsTrue(container.Get<Service2>(0).Test == 0);
        }
        
        [Test] public void NotConfiguringFactoryOfRegisteredService_CorrectlyInjectsDefaultService() { 
            m_DIBuilder.AddSingleton<Service2>(0);
            var container = m_DIBuilder.Build();
            
            Assert.IsTrue(container.Get<Service2>(0).Test == 0);
        }
        
        [Test] public void ConfiguringConfigurationOfRegisteredService_CorrectlyInjectsServiceWithConfiguration() {
            m_DIBuilder.AddSingleton<Service2>(0).WithConfiguration(service => { service.Test = 1; return service; }); 
            var container = m_DIBuilder.Build();
            
            Assert.IsTrue(container.Get<Service2>(0).Test == 1);
        }
        
        [Test] public void ConfiguringFactoryOfRegisteredService_CorrectlyInjectsServiceWithFactory() { 
            m_DIBuilder.AddSingleton<Service2>(0).WithFactory(() => { return new Service2 { Test = 1 }; }); 
            var container = m_DIBuilder.Build();
            
            // Service2_2 expects int value to be 2, by default it is 1, here it has been created with a value of 2. 
            Assert.IsTrue(container.Get<Service2>(0).Test == 1);

        }
        
        [Test] public void DisposingOfContainer_DisposesOfDisposableRegisteredServices() {
            m_DIBuilder.AddSingleton<Service3>(0);
            var container = m_DIBuilder.Build();
            var service = container.Get<IService3>(0);
            Assert.IsTrue(service.Disposed == false);
            container.Dispose();
            Assert.IsTrue(service.Disposed == true);
        }
    }
}