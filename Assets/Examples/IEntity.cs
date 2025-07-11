namespace Buttr.Core {
    /// <summary>
    /// Inherited for example Repository entities
    /// </summary>
    /// <typeparam name="TKey">The key for the entity in question</typeparam>
    public interface IEntity<out TKey> {
        TKey ID { get; }
    }
}