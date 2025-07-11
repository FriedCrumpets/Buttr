using System;
using System.Reflection;

namespace Buttr.Core {
    public abstract class ObjectResolverBase<TConcrete> : IObjectResolver {
        protected readonly Type[] requirements;
        protected readonly Func<object[], TConcrete> factory;

        protected ObjectResolverBase() {
            ConstructorInfo selectedConstructor = null;

            var constructors = typeof(TConcrete).GetConstructors();
            for (var i = 0; i < constructors.Length; i++) {
                if (constructors[i].IsPublic == false) { continue; }
                
                selectedConstructor = constructors[i];
                break;
            }

            if (selectedConstructor == null)
                throw new InvalidOperationException($"No public constructor found for type {typeof(TConcrete)}");

            var paramInfos = selectedConstructor.GetParameters();
            var paramCount = paramInfos.Length;
            requirements = paramCount == 0 ? Array.Empty<Type>() : new Type[paramCount];

            for (var i = 0; i < paramCount; i++) {
                requirements[i] = paramInfos[i].ParameterType;
            }

            factory = ObjectFactory.Create<TConcrete>(selectedConstructor, paramInfos);
        }

        public abstract bool IsResolved { get; }
        
        public abstract object Resolve();
    }
}