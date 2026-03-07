using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Buttr.Core;
using UnityEngine;

namespace Buttr.Unity {

    
    [Serializable]
    public sealed class ScriptableInjector {
        [SerializeField] private List<ScriptableObject> m_Objects;

        public void Inject(ApplicationBuilder builder) {
            foreach (var obj in m_Objects) {
                Inject__Internal (obj.GetType()) (builder, obj);
            }
        }

        private static Action<ApplicationBuilder, ScriptableObject> Inject__Internal(Type concreteType) {
            var builderParam = Expression.Parameter(
                typeof(ApplicationBuilder), "builder"
            );
            
            var objParam = Expression.Parameter(
                typeof(ScriptableObject), "obj"
            );

            var accessorExpr = Expression.Property(builderParam,  "Resolvers");
            var accessorType = typeof(IResolverCollection);

            var addSingletonMethod = accessorType
                .GetMethods()
                .First(m => m.Name == "AddSingleton" &&
                            m.IsGenericMethodDefinition &&
                            m.GetGenericArguments().Length == 1)
                .MakeGenericMethod(concreteType);

            var addCall = Expression.Call(
                accessorExpr, addSingletonMethod
            );

            var castExpr = Expression.Convert(objParam, concreteType);
            var factoryType = typeof(Func<>).MakeGenericType(concreteType);
            var factoryLambda = Expression.Lambda(factoryType, castExpr);

            var configurableType = typeof(IConfigurable<>)
                .MakeGenericType(concreteType);
            
            var withFactoryMethod = configurableType
                .GetMethod("WithFactory");

            var withFactoryCall = Expression.Call(
                addCall, withFactoryMethod, factoryLambda
            );

            var lambda = Expression.Lambda<Action<ApplicationBuilder, ScriptableObject>>(
                withFactoryCall, builderParam, objParam
            );

            return lambda.Compile();
        }
        
        public void Inject(IDIBuilder builder) {
            foreach (var obj in m_Objects) {
                InjectDI__Internal
                    (obj.GetType())
                    (builder, obj);
            }
        }

        private static Action<IDIBuilder, ScriptableObject> InjectDI__Internal(Type concreteType) {
            var builderParam = Expression.Parameter(
                typeof(IDIBuilder), "builder"
            );
            
            var objParam = Expression.Parameter(
                typeof(ScriptableObject), "obj"
            );

            var addSingletonMethod = typeof(IDIBuilder)
                .GetMethods()
                .First(m => m.Name == "AddSingleton" &&
                            m.IsGenericMethodDefinition &&
                            m.GetGenericArguments().Length == 1)
                .MakeGenericMethod(concreteType);

            var addCall = Expression.Call(
                builderParam, addSingletonMethod
            );

            var castExpr = Expression.Convert(objParam, concreteType);
            var factoryType = typeof(Func<>).MakeGenericType(concreteType);
            var factoryLambda = Expression.Lambda(factoryType, castExpr);

            var configurableType = typeof(IConfigurable<>)
                .MakeGenericType(concreteType);
            
            var withFactoryMethod = configurableType
                .GetMethod("WithFactory");

            var withFactoryCall = Expression.Call(
                addCall, withFactoryMethod, factoryLambda
            );

            var lambda = Expression.Lambda<Action<IDIBuilder, ScriptableObject>>(
                withFactoryCall, builderParam, objParam
            );

            return lambda.Compile();
        }
    }
}