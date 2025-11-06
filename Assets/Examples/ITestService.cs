using System;
using UnityEngine;

namespace Examples {
    public interface ITestService { }

    public sealed class TestService : ITestService { }

    public interface ITestService2 { }

    public sealed class TestService2 : ITestService2 {
        private readonly ITestService testService;

        public TestService2(ITestService testService) {
            this.testService = testService;
        }
    }
    
    public sealed class TestService3  {
        private readonly ITestService2 testService;

        public TestService3(ITestService2 testService) {
            this.testService = testService;
        }
    }


    public sealed class PositionPointRegistry {
        IDisposable Register(PositionPoint positionPoint) {
            return default;
        }
    }

    public readonly struct PositionPointID : IEquatable<PositionPointID> {
        private readonly string m_ID;

        private PositionPointID(string id) {
            m_ID = id;
        }

        public static implicit operator string(PositionPointID entity) => entity.m_ID;
        public static implicit operator PositionPointID(string id) => new(id);

        public override string ToString() {
            return m_ID;
        }

        public override int GetHashCode() {
            return (m_ID != null ? m_ID.GetHashCode() : 0);
        }

        public override bool Equals(object obj) {
            return obj is PositionPointID other && Equals(other);
        }

        public bool Equals(PositionPointID other) {
            return m_ID == other.m_ID;
        }

        public static bool operator ==(PositionPointID left, PositionPointID right) => left.Equals(right);
        public static bool operator !=(PositionPointID left, PositionPointID right) => !left.Equals(right);
    }

    public sealed class PositionPoint : MonoBehaviour {
        [SerializeField] private string m_ID;

        public PositionPointID ID => m_ID;

    }
}
