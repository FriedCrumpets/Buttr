namespace Buttr.Core {
    internal interface IObjectResolver {
        bool IsResolved { get; }
        object Resolve();
    }
}