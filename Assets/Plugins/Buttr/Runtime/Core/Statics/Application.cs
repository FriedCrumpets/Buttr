namespace Buttr.Core {
    public static class Application<T> {
        private static IObjectResolver s_Resolver;
        internal static void Set(IObjectResolver serviceResolver) => s_Resolver = serviceResolver;
        public static T Get() => (T)s_Resolver.Resolve();
    }
}