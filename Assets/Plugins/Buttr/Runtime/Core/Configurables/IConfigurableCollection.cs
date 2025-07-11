using System;

namespace Buttr.Core {
    public interface IConfigurableCollection {
        public IConfigurableCollection WithConfiguration<TConcrete>(Func<TConcrete, TConcrete> configuration);
        public IConfigurableCollection WithFactory<TConcrete>(Func<TConcrete> factory);
    }
}