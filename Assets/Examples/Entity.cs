namespace Buttr.Core {
    /// <summary>
    /// Example Entity. 
    /// </summary>
    /// <remarks>
    /// - We should be passing an ID through in the constructor
    /// - alternatively consider a ScriptableObject approach with ID's that are serialized.
    /// </remarks>
    public sealed class Entity : IEntity<Identifier> {
        public Identifier ID { get; } 
    }
}