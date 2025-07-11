using System;

namespace Buttr.Core {
    public interface IConfigurable<TConcrete> {
        IConfigurable<TConcrete> WithConfiguration(Func<TConcrete, TConcrete> configuration);
        IConfigurable<TConcrete> WithFactory(Func<TConcrete> factory);
    }
}