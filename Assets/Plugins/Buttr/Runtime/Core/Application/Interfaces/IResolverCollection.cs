namespace Buttr.Core {
    public interface IResolverCollection : IResolver {
        public IConfigurable<TConcrete> AddSingleton<TConcrete>();
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract;
        public IConfigurable<TConcrete> AddTransient<TConcrete>();
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract;
    }
}