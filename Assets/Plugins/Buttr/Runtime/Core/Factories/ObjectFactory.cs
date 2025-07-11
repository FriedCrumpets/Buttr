using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Buttr.Core {
    internal static class ObjectFactory {
        internal static Func<object[], TConcrete> Create<TConcrete>(ConstructorInfo constructor, ParameterInfo[] parameters) {
            var parametersParameter = Expression.Parameter(typeof(object[]), "args");
            
            var constructorParameters = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++) {
                var indexExpr = Expression.Constant(i);
                var arrayAccess = Expression.ArrayIndex(parametersParameter, indexExpr);
                var convertExpr = Expression.Convert(arrayAccess, parameters[i].ParameterType);
                constructorParameters[i] = convertExpr;
            }

            var newExpression = Expression.New(constructor, constructorParameters);
            var lambda = Expression.Lambda<Func<object[], TConcrete>>(newExpression, parametersParameter);
            return lambda.Compile();
        }
    }
}