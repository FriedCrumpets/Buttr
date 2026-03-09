using System;

namespace Buttr.Core {
    public interface IDIBuilder<in TID> {
        Type Type { get; }
        
        IConfigurable<TConcrete> AddTransient<TConcrete>(TID id);
        IConfigurable<TConcrete> AddSingleton<TConcrete>(TID id);
        IDIContainer<TID> Build();
    }
    
    public interface IDIBuilder {
        IResolverCollection Resolvers { get; }
        IResolverCollection Hidden { get; }
        IConfigurable<TConcrete> AddTransient<TConcrete>();
        IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract;
        IConfigurable<TConcrete> AddSingleton<TConcrete>();
        IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract;
        IDIContainer Build();
    }
}