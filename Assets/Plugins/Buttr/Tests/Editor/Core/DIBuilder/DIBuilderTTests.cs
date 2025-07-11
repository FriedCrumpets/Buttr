using System;
using Buttr.Core;
using NUnit.Framework;

namespace Buttr.Tests.Editor.DIBuilder {
    public sealed class DIBuilderTTests {
        interface IService0 { }
        class Service0 : IService0 { }
        
        readonly struct TestID : IEquatable<TestID> {
            private TestID(string id) { ID = id; }

            public string ID { get; }

            public static implicit operator string(TestID entity) => entity.ID;
            public static implicit operator TestID(string id) => new(id);

            public override string ToString() {
                return ID;
            }
        
            public override int GetHashCode() {
                return (ID != null ? ID.GetHashCode() : 0);
            }

            public override bool Equals(object obj) {
                return obj is TestID other && Equals(other);
            }
        
            public bool Equals(TestID other) {
                return ID == other.ID;
            }

            public static bool operator ==(TestID left, TestID right) => left.Equals(right);
            public static bool operator !=(TestID left, TestID right) => !left.Equals(right);
        }
        
        private IDIBuilder<TestID> m_Builder;
        
        [SetUp]
        public void Setup() {
            m_Builder = new DIBuilder<TestID>();
        }
        
        [Test]
        public void DIBuilder_AddTransient_TConcrete_ReturnsConfigurableOfTypeTConcrete() {
            var configurable = m_Builder.AddTransient<Service0>("0");
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }

        [Test]
        public void DIBuilder_AddSingleton_TConcrete_ReturnsConfigurableOfTypeTConcrete() { 
            var configurable = m_Builder.AddSingleton<Service0>("0");
            
            // confirm the configurable is of the correct type
            Assert.IsTrue(configurable is IConfigurable<Service0>);
        }
        
        [Test]
        public void DIBuilder_AddTheSameSingleton_TConcrete_TwiceWithSameIDThrows() {
            m_Builder.AddSingleton<Service0>("0");
            
            // can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddSingleton<Service0>("0"));
        }
        
        [Test]
        public void DIBuilder_AddTheSameSingleton_TConcrete_TwiceWithDifferentID_DoesNotThrow() {
            m_Builder.AddSingleton<Service0>("0");
            
            //  can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddSingleton<Service0>("1"));
        }

        [Test]
        public void DIBuilder_AddTheSameTransient_TConcrete_TwiceWithSameID_DoesNotThrow() {
            m_Builder.AddTransient<Service0>("0");
            
            //  can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddTransient<Service0>("0"));
        }
        
        [Test]
        public void DIBuilder_AddTheSameTransient_TConcrete_TwiceWithDifferentID_DoesNotThrow() {
            m_Builder.AddTransient<Service0>("0");
            
            // can add the same resolver twice
            Assert.DoesNotThrow(() => m_Builder.AddTransient<Service0>("1"));
        }

        [Test]
        public void DIBuilder_Build_DoesNotThrow() {
            m_Builder.AddTransient<Service0>("0");

            Assert.DoesNotThrow(() => m_Builder.Build(), "Builder should not throw with 0 or more services provided");
        }

        [Test]
        public void DIBuilder_Builder_ReturnsIDIContainer() { 
            m_Builder.AddTransient<Service0>("0");
            var container = m_Builder.Build();
            
            Assert.IsTrue(container is IDIContainer<TestID>);
        }
    }
}