using System;

namespace Buttr.Core {
    /// <summary>
    /// >> TEMPLATE FOR Entity ID's
    /// </summary>
    /// <remarks>
    /// Think Domain Driven Design with unique Identifiers when crafting Identifiers
    /// - Use Constructors to split strings into identifiable IDs
    /// - An Identifier can contain multiple other identifiers split from it's parameter(s)
    /// - implicit conversion is your friend with identifiers. Especially those with string based constructors
    /// - An Identifiers ID should be the raw result of constructor input. It can also be a wrapper for a more refined class
    /// </remarks>
    public readonly struct Identifier : IEquatable<Identifier> {
        private Identifier(string id) { ID = id; }

        public string ID { get; }

        public static implicit operator string(Identifier entity) => entity.ID;
        public static implicit operator Identifier(string id) => new(id);

        public override string ToString() {
            return ID;
        }
        
        public override int GetHashCode() {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public override bool Equals(object obj) {
            return obj is Identifier other && Equals(other);
        }
        
        public bool Equals(Identifier other) {
            return ID == other.ID;
        }

        public static bool operator ==(Identifier left, Identifier right) => left.Equals(right);
        public static bool operator !=(Identifier left, Identifier right) => !left.Equals(right);
    }
}