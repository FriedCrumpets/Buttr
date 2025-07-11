using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using UnityEngine;

// ReSharper disable UnusedParameter.Local
// ReSharper disable StaticMemberInGenericType
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable ObjectCreationAsStatement
// ReSharper disable SuspiciousTypeConversion.Global

namespace Buttr.Core.Complete {
    public interface IObjectResolver {
        object Resolve();
    }

    internal static class ObjectFactory {
        internal static Func<object[], TConcrete> Create<TConcrete>(ConstructorInfo constructor) {
            var parametersParameter = Expression.Parameter(typeof(object[]), "args");

            var constructorParameters = constructor.GetParameters()
                .Select((param, index) =>
                    Expression.Convert(
                        Expression.ArrayIndex(parametersParameter, Expression.Constant(index)),
                        param.ParameterType))
                .ToArray<Expression>();

            var newExpression = Expression.New(constructor, constructorParameters);
            return Expression.Lambda<Func<object[], TConcrete>>(newExpression, parametersParameter).Compile();
        }
    }

    public abstract class ObjectResolverBase<TConcrete> : IObjectResolver {
        protected readonly List<Type> requirements = new();
        protected readonly Func<object[], TConcrete> factory;

        protected ObjectResolverBase() {
            var constructor = typeof(TConcrete).GetConstructors().FirstOrDefault();
            
            if (constructor == null)
                throw new InvalidOperationException($"No public constructor found for type {typeof(TConcrete)}");

            foreach (var parameter in constructor.GetParameters()) {
                requirements.Add(parameter.ParameterType);
            }

            factory = ObjectFactory.Create<TConcrete>(constructor);
        }

        public abstract object Resolve();
    }

    internal sealed class TransientResolverInternal<TConcrete> : ObjectResolverBase<TConcrete> {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;

        public TransientResolverInternal(Dictionary<Type, IObjectResolver> registry, Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Registry = registry;
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }

        public override object Resolve() {
            (var dependencies, var remaining) = m_Registry.GetDependencies(requirements);
            dependencies.AddRange(StaticServiceResolver.GetDependencies(remaining));
            var dArray = dependencies.ToArray();
            
            if (dArray.TryValidate(requirements) == false) {
                throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)}) \r\n Required Dependencies :: {string.Join(", ", requirements)}");
            }

            return m_Configuration(m_FactoryOverride == null
                ? factory(dArray)
                : m_FactoryOverride());
        }
    }
    
    internal sealed class SingletonResolverInternal<TConcrete> : ObjectResolverBase<TConcrete> {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;

        private TConcrete m_Instance;
        
        public SingletonResolverInternal(Dictionary<Type, IObjectResolver> registry, Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Registry = registry;
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }

        public override object Resolve() {
            if (m_Instance != null) return m_Instance;
            
            var (dependencies, remaining) = m_Registry.GetDependencies(requirements);
            dependencies.AddRange(StaticServiceResolver.GetDependencies(remaining));
            var dArray = dependencies.ToArray();
            if (dArray.TryValidate(requirements) == false) {
                throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)}) \r\n Required Dependencies :: {string.Join(", ", requirements)}");
            }

            m_Instance = m_Configuration(m_FactoryOverride == null
                ? factory(dArray)
                : m_FactoryOverride());

            return m_Instance;
        }
    }
    
    internal sealed class IDTransientResolverInternal<TConcrete> : ObjectResolverBase<TConcrete> {
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;

        public IDTransientResolverInternal(Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }

        public override object Resolve() {
            var dependencies = StaticServiceResolver.GetDependencies(requirements);
            if (dependencies.TryValidate(requirements) == false) {
                throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)}) \r\n Required Dependencies :: {string.Join(", ", requirements)}");
            }

            return m_Configuration(m_FactoryOverride == null
                ? factory(dependencies)
                : m_FactoryOverride());
        }
    }
    
    internal sealed class IDSingletonResolverInternal<TConcrete> : ObjectResolverBase<TConcrete> {
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;

        private TConcrete m_Instance;
        
        public IDSingletonResolverInternal(Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }

        public override object Resolve() {
            if (m_Instance != null) return m_Instance;
            
            var dependencies = StaticServiceResolver.GetDependencies(requirements);
            if (dependencies.TryValidate(requirements) == false) {
                throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)}) \r\n Required Dependencies :: {string.Join(", ", requirements)}");
            }

            m_Instance = m_Configuration(m_FactoryOverride == null
                ? factory(dependencies)
                : m_FactoryOverride());

            return m_Instance;
        }
    }

    public interface IHidden { }

    public interface IDIBuilder {
        IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>();
        IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>();
        IDIContainer Build();
    }

    internal static class DIBuilderUtility {
        public static (List<object> dependencies, List<Type> remaining) GetDependencies(this Dictionary<Type, IObjectResolver> registry, List<Type> requirements) {
            var dependencies = new List<object>();
            var remaining = new List<Type>();
    
            foreach (var type in requirements) {
                if (registry.TryGetValue(type, out var value))
                    dependencies.Add(value.Resolve());
                else 
                    remaining.Add(type);
            }
            
            return (dependencies, remaining);
        }
    }

    public class DIBuilder : IDIBuilder {
        private readonly Dictionary<Type, IObjectResolver> m_Registry = new();
        private readonly List<IResolver> m_Resolvers = new();
        
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() {
            var resolver =  new TransientObjectResolver<TAbstract, TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() {
            var resolver =  new SingletonObjectResolver<TAbstract, TConcrete>(m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IDIContainer Build() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();

            return new DIContainer(m_Registry);
        }
    }

    public interface IDIContainer : IDisposable {
        TAbstract Get<TAbstract>();
    }

    internal sealed class DIContainer : IDIContainer {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        
        internal DIContainer(Dictionary<Type, IObjectResolver> registry) {
            m_Registry = registry;
        }
        
        public TAbstract Get<TAbstract>() {
            if (m_Registry.TryGetValue(typeof(TAbstract), out var resolver)) {
                if (resolver is not IHidden) {
                    return (TAbstract)resolver.Resolve();
                }
            }

            return default;
        }
        
        public void Dispose() {
            foreach (var resolver in m_Registry.Values) {
                if(resolver.Resolve() is IDisposable disposable) disposable.Dispose();
            }
            
            m_Registry.Clear();
        }
    }
    
    public interface IContainerDefinition { } // tag interface

    public static class Container<TContainerDefinition> where TContainerDefinition : IContainerDefinition {
        private static IDIContainer m_Container;

        internal static void Set(IDIBuilder builder) => m_Container = builder.Build();
        internal static IDIContainer Get() => m_Container;

        public static TAbstract Get<TAbstract>() => m_Container.Get<TAbstract>();
    }
    
    internal sealed class ContainerResolver<TContainerDefinition> : IResolver, IConfigurable<IDIBuilder> where TContainerDefinition : IContainerDefinition {
        private Func<IDIBuilder, IDIBuilder> m_Configuration = ConfigurationFactory.Empty<IDIBuilder>();
        private Func<IDIBuilder> m_Factory;

        public void Resolve() {
            var builder = m_Factory == null ? new DIBuilder() : m_Factory();
            builder = m_Configuration(builder);
            Container<TContainerDefinition>.Set(builder);
        }
        
        public void Dispose() { 
            Container<TContainerDefinition>.Get().Dispose();
            Container<TContainerDefinition>.Set(null);
        }
        
        IConfigurable<IDIBuilder> IConfigurable<IDIBuilder>.WithConfiguration(Func<IDIBuilder, IDIBuilder> configuration) {
            m_Configuration = configuration;
            return this;
        }
        
        IConfigurable<IDIBuilder> IConfigurable<IDIBuilder>.WithFactory(Func<IDIBuilder> factory) {
            m_Factory = factory;
            return this;
        }
    }

    internal interface IContainerResolver : IResolver {
        IConfigurable<IDIBuilder> DefineContainer<TContainerDefinition>() where TContainerDefinition : IContainerDefinition;
    }
    
    public sealed class ApplicationContainer : IContainerResolver {
        private IResolver m_Resolver;
        
        public IConfigurable<IDIBuilder> DefineContainer<TContainerDefinition>() where TContainerDefinition : IContainerDefinition {
            var resolver = new ContainerResolver<TContainerDefinition>();
            m_Resolver = resolver;
            return resolver;
        }
        
        public void Resolve() {
            m_Resolver.Resolve();
        }
        
        public void Dispose() {
            m_Resolver.Dispose();
        }
    }
    
    /*
        public static IConfigurableCollection UseX(this UnityApplicationBuilder builder) {
            var b = builder.DefineContainer<X>();
            return new ConfigurableCollection()
                .Register(b.AddTransient)
        }
     */

    public interface IContainerCollection : IResolver {
        IConfigurable<IDIBuilder> DefineContainer<TContainerDefintion>() where TContainerDefintion : IContainerDefinition;
    }
    
    public sealed class ApplicationContainerCollection : IContainerCollection {
        private readonly List<IResolver> m_Resolvers = new();
        
        public IConfigurable<IDIBuilder> DefineContainer<TContainerDefintion>() where TContainerDefintion : IContainerDefinition {
            var resolver = new ApplicationContainer();
            m_Resolvers.Add(resolver);
            return resolver.DefineContainer<TContainerDefintion>();
        }
        
        public void Resolve() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();
        }
        
        public void Dispose() {
            foreach(var resolver in m_Resolvers) resolver.Dispose();
        }
    }
    
    public sealed class UnityContainer : ScriptableObject, IResolver {
        [SerializeField, Tooltip("Must inherit from IContainerDefinition")] private ScriptableObject m_ContainerDefinition;
        [SerializeField] private List<UnityObjectResolver> m_Resolvers;
        private ApplicationContainer m_Container;
        private bool m_Resolved;

        public void Resolve() {
            if (m_ContainerDefinition is not IContainerDefinition) {
                throw new ArgumentException(nameof(m_ContainerDefinition), $"The Container Definition for a {typeof(UnityContainer)} must be of type IContainerDefinition");
            }

            var configurable = m_Container.GenerateContainerConstructor(m_ContainerDefinition.GetType())(m_Container);
            configurable.WithConfiguration(builder => {
                foreach (var resolver in m_Resolvers) {
                    if(resolver.Object is not (IController or IService or IConfiguration or IRepository))
                        throw new ArgumentNullException(nameof(resolver.Object), $"All ScriptableObjects listed in a {typeof(UnityContainer)} must be of type IHidden");
                    builder.GenerateConstructorFor(resolver.Object, resolver.Type)(builder, resolver.Object);
                }
                return builder;
            });
        }
        
        public void Dispose() {
            m_Container.Dispose();
            m_Resolved = false;
        }
    }
    
    public delegate IConfigurable<IDIBuilder> ContainerDefinition(ApplicationContainer container);
    
    /*
     *  public static void UseX(this UnityApplicationBuilder builder) {
     *      var container = new ApplicationContainer();
     *      container.DefineContainer<T>()
     *          .WithConfiguration(b => {
     *              b.AddTransient<T, TT>();
     *              b.AddSingleton<T, TT>();
     *          }
     *      );
     *      builder.AddContainer(container);
     *  }
     * 
     * var builder = new DIBuilder();
     * var configurableCollection = new ConfigurableCollection()
     *      .Register(builder.AddSingleton<IAssetService, AssetService>())
     *      .Register(builder.AddSingleton<IAddressableService, AddressableService>());
     * 
     * var container = new ApplicationContainer()
     * container.DefineContainer<IAddressables>(builder)
     *      .WithFactory(() => builder);
     * return (container, configurableCollection);
     */

    public interface IDIBuilder<in TID> {
        Type Type { get; }
        
        IConfigurable<TConcrete> AddTransient<TConcrete>(TID id);
        IConfigurable<TConcrete> AddSingleton<TConcrete>(TID id);
        IDIContainer<TID> Build();
    }
    
    public class DIBuilder<TID> : IDIBuilder<TID> {
        private readonly Dictionary<TID, IObjectResolver> m_Registry = new();
        private readonly List<IResolver> m_Resolvers = new();
        public Type Type => typeof(TID);
        
        public IConfigurable<TConcrete> AddTransient<TConcrete>(TID id) {
            var resolver =  new IDTransientObjectResolver<TID, TConcrete>(id, m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IConfigurable<TConcrete> AddSingleton<TConcrete>(TID id) {
            var resolver =  new IDSingletonObjectResolver<TID, TConcrete>(id, m_Registry);
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public IDIContainer<TID> Build() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();

            return new DIContainer<TID>(m_Registry);
        }
    }
    
    public interface IDIContainer<in TID> : IDisposable {
        Type Type { get; }

        TConcrete Get<TConcrete>(TID id);
    }

    public class DIContainer<TID> : IDIContainer<TID> {
        private readonly Dictionary<TID, IObjectResolver> m_Registry;
        
        internal DIContainer(Dictionary<TID, IObjectResolver> registry) {
            m_Registry = registry;
        }
        
        public Type Type => typeof(TID);
        
        public TConcrete Get<TConcrete>(TID id) {
            if (m_Registry.TryGetValue(id, out var resolver))
                return (TConcrete)resolver.Resolve();

            return default;
        }
        
        public void Dispose() {
            foreach (var resolver in m_Registry.Values) {
                if (resolver.Resolve() is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
            
            m_Registry.Clear();
        }
    }
    
    internal sealed class TransientObjectResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;
    
        internal TransientObjectResolver(Dictionary<Type, IObjectResolver> registry) {
            m_Registry = registry;
        }
        
        public void Resolve() {
            m_Registry[typeof(TAbstract)] = new TransientResolverInternal<TConcrete>(m_Registry, m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Registry[typeof(TAbstract)].Resolve() is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }
    
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    } 
    
    internal sealed class SingletonObjectResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> {
        private readonly Dictionary<Type, IObjectResolver> m_Registry;
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;
    
        internal SingletonObjectResolver(Dictionary<Type, IObjectResolver> registry) {
            m_Registry = registry;
        }
        
        public void Resolve() {
            m_Registry[typeof(TAbstract)] = new SingletonResolverInternal<TConcrete>(m_Registry, m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Registry[typeof(TAbstract)].Resolve() is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }
    
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    } 
    
    // a generic object that allows for resolving by ID
    internal sealed class IDTransientObjectResolver<TID, TConcrete> : IResolver, IConfigurable<TConcrete> {
        private readonly TID m_ID;
        private readonly Dictionary<TID, IObjectResolver> m_Registry;
        
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;

        internal IDTransientObjectResolver(TID id, Dictionary<TID, IObjectResolver> registry) {
            m_ID = id;
            m_Registry = registry;
        }
        
        public void Resolve() {
            m_Registry[m_ID] = new IDTransientResolverInternal<TConcrete>(m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Registry[m_ID].Resolve() is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }
    
    internal sealed class IDSingletonObjectResolver<TID, TConcrete> : IResolver, IConfigurable<TConcrete> {
        private readonly TID m_ID;
        private readonly Dictionary<TID, IObjectResolver> m_Registry;
        
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;

        internal IDSingletonObjectResolver(TID id, Dictionary<TID, IObjectResolver> registry) {
            m_ID = id;
            m_Registry = registry;
        }
        
        public void Resolve() {
            m_Registry[m_ID] = new IDSingletonResolverInternal<TConcrete>(m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Registry[m_ID].Resolve() is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }

    internal abstract class StaticServiceResolverBase<TAbstract, TConcrete> : IObjectResolver, IDisposable {
        protected readonly List<Type> requirements = new();
        protected readonly Func<object[], TConcrete> factory;
        
        private readonly IDisposable m_Registration;

        protected StaticServiceResolverBase() {
            var constructor = typeof(TConcrete).GetConstructors().FirstOrDefault();
            
            if (constructor == null)
                throw new InvalidOperationException($"No public constructor found for type {typeof(TConcrete)}");

            foreach (var parameter in constructor.GetParameters()) {
                requirements.Add(parameter.ParameterType);
            }

            factory = ObjectFactory.Create<TConcrete>(constructor);
            m_Registration = StaticServiceResolver.Register<TAbstract>(this);
        }

        public abstract object Resolve();
        
        public void Dispose() {
            m_Registration?.Dispose();
        }
    }

    internal static class ServiceResolverUtilities {
        private static readonly List<Type> s_RemainingRequirements = new();
        
        public static bool TryValidate(this object[] foundDependencies, List<Type> requirements) {
            if (foundDependencies.Length != requirements.Count) return false;
            
            s_RemainingRequirements.Clear();
            s_RemainingRequirements.AddRange(requirements);
            for (var i = 0; i < foundDependencies.Length; i++) {
                var dependencyType = foundDependencies[i].GetType();
                var requiredType = requirements[i];

                if (requiredType.IsAssignableFrom(dependencyType)) {
                    s_RemainingRequirements.Remove(requiredType);
                }
                else {
                    Debug.LogWarning($"Dependency of type {dependencyType} does not satisfy required type {requiredType}.");
                }
            }

            return s_RemainingRequirements.Count == 0;
        }
    }

    internal sealed class Transient<TAbstract, TConcrete> : StaticServiceResolverBase<TAbstract, TConcrete> {
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;

        internal Transient(Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }

        public override object Resolve() {
            var dependencies = StaticServiceResolver.GetDependencies(requirements);
            if (dependencies.TryValidate(requirements) == false) {
                throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)}) \r\n Required Dependencies :: {string.Join(", ", requirements)}");
            }
            
            return m_Configuration(m_FactoryOverride == null 
                ? factory(dependencies) 
                : m_FactoryOverride());
        }
    }

    internal sealed class Singleton<TAbstract, TConcrete> : StaticServiceResolverBase<TAbstract, TConcrete>, IObjectResolver {
        private object m_Concrete;
        private readonly Func<TConcrete, TConcrete> m_Configuration;
        private readonly Func<TConcrete> m_FactoryOverride;

        internal Singleton(Func<TConcrete, TConcrete> configuration, Func<TConcrete> factoryOverride) {
            m_Configuration = configuration;
            m_FactoryOverride = factoryOverride;
        }

        public override object Resolve() {
            if (null != m_Concrete) return m_Concrete;
            
            var dependencies = StaticServiceResolver.GetDependencies(requirements);
            if (dependencies.TryValidate(requirements) == false) {
                throw new ObjectResolverException($"Unable to locate all dependencies for {typeof(TConcrete)}) \r\n Required Dependencies :: {string.Join(", ", requirements)}");
            }
            
            m_Concrete = m_Configuration(m_FactoryOverride == null 
                ? factory(dependencies) 
                : m_FactoryOverride());

            return m_Concrete;
        }
    }

    public interface IEntity<out TKey> {
        TKey ID { get; }
    }

    /// <summary>
    /// >> TEMPLATE FOR Entity ID's
    /// </summary>
    /// <remarks>
    /// Think Domain Driven Design with unique Identifiers when crafting Identifiers
    /// - Use Constructors to split strings into identifiable IDs
    /// - An Identifier can contain multiple other identifiers split from it's parameter(s)
    /// - implicit conversion is your friend with identifiers. Especially those with string based constructors
    /// - An Identifiers ID should be the raw result of it's input. It can also be a wrapper for a more refined class
    /// </remarks>
    public readonly struct Identifier : IEquatable<Identifier> {
        private Identifier(string id) { ID = id; }

        public string ID { get; }

        public static implicit operator string(Identifier entity) => entity.ID;
        public static implicit operator Identifier(string id) => new(id);

        public override string ToString() {
            return ID;
        }
        
        public override int GetHashCode() {
            return (ID != null ? ID.GetHashCode() : 0);
        }

        public override bool Equals(object obj) {
            return obj is Identifier other && Equals(other);
        }
        
        public bool Equals(Identifier other) {
            return ID == other.ID;
        }

        public static bool operator ==(Identifier left, Identifier right) => left.Equals(right);
        public static bool operator !=(Identifier left, Identifier right) => !left.Equals(right);
    }
    
    /// <summary>
    /// Example Entity. 
    /// </summary>
    /// <remarks>
    /// - We should be passing an ID through in the constructor
    /// - alternatively consider a ScriptableObject approach with ID's that are serialized.
    /// </remarks>
    public sealed class Entity : IEntity<Identifier> {
        public Identifier ID { get; } 
    }

    public interface IRepository { }

    /// <summary>
    /// Repositories
    /// </summary>
    /// <remarks>
    /// This may seem redundant especially with the example implementations being simple dictionaries obviously this isn't a representation of how this should work.
    /// 
    /// These will want to be
    /// - tied to a central database and/or save load system.
    /// - work alongside services to simply store downloaded data via a key
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public interface IRepository<in TKey, TData> : IRepository where TData : IEntity<TKey> {
        ICollection<TData> RetrieveAll();
        IEnumerable<TData> RetrieveByCondition(Func<TData, bool> condition);
        void Create(TData entity);
        TData Read(TKey id);
        void Update(TData entity);
        bool Delete(TData entity);
        bool Delete(TKey id);
        void Clear();
    }

    public abstract class RepositoryBase<TKey, TData> : IRepository<TKey, TData> where TData : IEntity<TKey> {
        protected readonly object @lock = new();
        protected readonly IDictionary<TKey, TData> dataStore = new Dictionary<TKey, TData>();

        public ICollection<TData> RetrieveAll() {
            lock (@lock) return dataStore.Values;
        }

        public IEnumerable<TData> RetrieveByCondition(Func<TData, bool> condition) {
            lock (@lock) return dataStore.Values.Where(condition.Invoke);
        }

        public void Create(TData entity) {
            lock (@lock) dataStore[entity.ID] = entity;
        }

        public TData Read(TKey id) {
            lock (@lock) return dataStore.TryGetValue(id, out var data) ? data : default;
        }

        public void Update(TData entity) {
            lock (@lock) {
                if (dataStore.ContainsKey(entity.ID))
                    dataStore[entity.ID] = entity;
            }
        }

        public bool Delete(TData entity) {
            lock (@lock) {
                return dataStore.Remove(entity.ID);
            }
        }

        public bool Delete(TKey id) {
            lock (@lock) {
                return dataStore.Remove(id);
            }
        }

        public void Clear() {
            lock (@lock) dataStore.Clear();
        }
    }
    
    public interface IAsyncRepository<in TKey, TData> : IRepository where TData : IEntity<TKey> {
        Awaitable<ICollection<TData>> RetrieveAllAsync(CancellationToken token);
        Awaitable<IEnumerable<TData>> RetrieveByConditionAsync(Func<TData, bool> condition, CancellationToken token);
        Awaitable CreateAsync(TData entity, CancellationToken token);
        Awaitable<TData> ReadAsync(TKey id, CancellationToken token);
        Awaitable<bool> UpdateAsync(TData entity, CancellationToken token);
        Awaitable<bool> DeleteAsync(TData entity, CancellationToken token);
        Awaitable<bool> DeleteAsync(TKey id, CancellationToken token);
        Awaitable<bool> ClearAsync(CancellationToken token);
    }
    
    public abstract class AsyncRepositoryBase<TKey, TData> : IAsyncRepository<TKey, TData> where TData : IEntity<TKey> {
        protected readonly object @lock = new();
        protected readonly IDictionary<TKey, TData> dataStore = new Dictionary<TKey, TData>();

        public Awaitable<ICollection<TData>> RetrieveAllAsync(CancellationToken token) {
            lock (@lock) return AwaitableUtility.FromResult(dataStore.Values);
        }

        public Awaitable<IEnumerable<TData>> RetrieveByConditionAsync(Func<TData, bool> condition, CancellationToken token) {
            lock (@lock) return AwaitableUtility.FromResult(dataStore.Values.Where(condition.Invoke));
        }

        public Awaitable CreateAsync(TData entity, CancellationToken token) {
            lock (@lock) dataStore[entity.ID] = entity;
            return AwaitableUtility.CompletedTask;
        }

        public Awaitable<TData> ReadAsync(TKey id, CancellationToken token) {
            lock (@lock) return AwaitableUtility.FromResult(dataStore.TryGetValue(id, out var data) ? data : default);
        }

        public Awaitable<bool> UpdateAsync(TData entity, CancellationToken token) {
            lock (@lock) {
                var exists = dataStore.ContainsKey(entity.ID);
                
                if (exists) {
                    dataStore[entity.ID] = entity;
                }

                return AwaitableUtility.FromResult(exists);
            }
        }

        public Awaitable<bool> DeleteAsync(TData entity, CancellationToken token) {
            lock (@lock) {
                return AwaitableUtility.FromResult(dataStore.Remove(entity.ID));
            }
        }

        public Awaitable<bool> DeleteAsync(TKey id, CancellationToken token) {
            lock (@lock) {
                return AwaitableUtility.FromResult(dataStore.Remove(id));
            }
        }

        public Awaitable<bool> ClearAsync(CancellationToken token) {
            lock (@lock) dataStore.Clear();
            return AwaitableUtility.FromResult(true);
        }
    }

    public static class AwaitableUtility {
        public static Awaitable<T> FromResult<T>(T result) {
            var acs = new AwaitableCompletionSource<T>();
            acs.SetResult(result);
            return acs.Awaitable;
        }

        public static Awaitable CompletedTask {
            get { return new AwaitableCompletionSource().Awaitable; }
        }
    }

    internal static class StaticServiceResolver {
        private static readonly Dictionary<Type, IObjectResolver> s_Services;

        static StaticServiceResolver() => s_Services = new();

        public static object[] GetDependencies(List<Type> requirements) {
            var dependencies = new List<object>();

            foreach (var type in requirements) {
                if (s_Services.TryGetValue(type, out var value))
                    dependencies.Add(value.Resolve());
            }
            
            return dependencies.ToArray();
        }

        public static IDisposable Register<T>(IObjectResolver resolver) {
            return new Registration(typeof(T), resolver);
        }

        private sealed class Registration : IDisposable {
            private readonly Type m_Type;
            private IObjectResolver m_Resolver;
            
            public Registration(Type type, IObjectResolver resolver) {
                m_Type = type;
                if (s_Services.TryAdd(type, resolver) == false) {
                    throw new ObjectResolverException($"Failed to add resolver to services, Has {type.Name} already been added?");
                }
            }
            
            public void Dispose() {
                s_Services.Remove(m_Type);
            }
        }
    }

    public sealed class ObjectResolverException : Exception {
        public ObjectResolverException(string message) : base(message) { }
    }

    public interface IService { }
    
    public static class Service<T> where T : IService {
        private static IObjectResolver s_ServiceResolver;
        internal static void Set(IObjectResolver serviceResolver) => s_ServiceResolver = serviceResolver;
        public static T Get() => (T)s_ServiceResolver.Resolve();
    }
    
    public interface IController { }
    
    public static class Controller<T> where T : IController {
        private static IObjectResolver s_ControllerResolver;
        internal static void Set(IObjectResolver serviceResolver) => s_ControllerResolver = serviceResolver;
        public static T Get() => (T)s_ControllerResolver.Resolve();
    }
    
    public interface IConfiguration { }
    
    public static class Configuration<T> where T : IConfiguration {
        private static IObjectResolver s_ControllerResolver;
        internal static void Set(IObjectResolver serviceResolver) => s_ControllerResolver = serviceResolver;
        public static T Get() => (T)s_ControllerResolver.Resolve();
    }
    
    public interface IResolver : IDisposable {
        void Resolve();
    }
    
    public interface IConfigurable<TConcrete> {
        IConfigurable<TConcrete> WithConfiguration(Func<TConcrete, TConcrete> configuration);
        IConfigurable<TConcrete> WithFactory(Func<TConcrete> factory);
    }

    public interface IConfigurableCollection {
        public IConfigurableCollection WithConfiguration<TConcrete>(Func<TConcrete, TConcrete> configuration);
        public IConfigurableCollection WithFactory<TConcrete>(Func<TConcrete> factory);
    }

    public sealed class ConfigurableCollection : IConfigurableCollection {
        private readonly Dictionary<Type, object> m_Configurables = new();
        
        public ConfigurableCollection Register<TConcrete>(IConfigurable<TConcrete> configurable) {
            m_Configurables[typeof(TConcrete)] = configurable;
            return this;
        }
        
        public IConfigurableCollection WithConfiguration<TConcrete>(Func<TConcrete, TConcrete> configuration) {
            if (m_Configurables.TryGetValue(typeof(TConcrete), out var configurable)) {
                var typedConfigurable = (IConfigurable<TConcrete>)configurable;
                typedConfigurable.WithConfiguration(configuration);
            }
            return this;
        }

        public IConfigurableCollection WithFactory<TConcrete>(Func<TConcrete> factory) {
            if (m_Configurables.TryGetValue(typeof(TConcrete), out var configurable)) {
                var typedConfigurable = (IConfigurable<TConcrete>)configurable;
                typedConfigurable.WithFactory(factory);
            }
            return this;
        }
    }
    
    internal static class ConfigurationFactory {
        public static Func<TConcrete, TConcrete> Empty<TConcrete>() {
            return t => t;
        } 
    }

    internal static class Factory {
        public static Func<TConcrete> Empty<TConcrete>() => null;
    }

    internal sealed class StaticServiceTransientResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> where TAbstract : IService {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;

        private Transient<TAbstract, TConcrete> m_Transient;
        
        public void Resolve() {
            m_Transient = new Transient<TAbstract, TConcrete>(m_Configuration, m_Factory);
            Service<TAbstract>.Set(m_Transient);
        }

        public void Dispose() {
            m_Transient.Dispose();
            Service<TAbstract>.Set(null);
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }
    
    internal sealed class StaticControllerTransientResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> where TAbstract : IController {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;
        
        private Transient<TAbstract, TConcrete> m_Transient;
        
        public void Resolve() {
            m_Transient = new Transient<TAbstract, TConcrete>(m_Configuration, m_Factory);
            Controller<TAbstract>.Set(m_Transient);
        }

        public void Dispose() {
            m_Transient.Dispose();
            Controller<TAbstract>.Set(null);
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }
    
    internal sealed class StaticConfigurationResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> where TAbstract : IConfiguration {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;
        
        private Transient<TAbstract, TConcrete> m_Transient;
        
        public void Resolve() {
            m_Transient = new Transient<TAbstract, TConcrete>(m_Configuration, m_Factory);
            Configuration<TAbstract>.Set(m_Transient);
        }

        public void Dispose() {
            m_Transient.Dispose();
            Configuration<TAbstract>.Set(null);
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }
    
    internal sealed class HiddenStaticTransientResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;

        private Transient<TAbstract, TConcrete> m_Transient;
        
        public void Resolve() {
            m_Transient = new Transient<TAbstract, TConcrete>(m_Configuration, m_Factory);
        }

        public void Dispose() {
            m_Transient.Dispose();
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }

        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }

    public interface IScopeDefinition { } // tag interface for definition.
    public interface IScopeContext { } // tag interface, injected when creating a new scope

    public interface IScopeBuilder {
        IDIBuilder WithContext<TAbstractContext, TConcreteContext>(TConcreteContext context);
    }
    
    internal sealed class ScopeBuilder : IScopeBuilder {
        private readonly IDIBuilder m_Builder;

        public ScopeBuilder(IDIBuilder builder) {
            m_Builder = builder;
        }
        
        public IDIBuilder WithContext<TAbstractContext, TConcreteContext>(TConcreteContext context) {
            m_Builder.AddSingleton<TAbstractContext, TConcreteContext>()
                .WithFactory(() => context);
            return m_Builder;
        }
    }
    
    /*
     * pseudo
     * builder.Scopes.DefineScope<ScopeDefinition, ScopeContext>((builder) => {
     *   builder.Add<IScopedService, ScopedService>()
     *      .WithConfiguration(configuration => { } );
     *      .WithFactory(() => new ScopedService()); // context is not available to scoped services created via a factory.
     * });
     * 
     * using(var scope = Scope<ScopeDefinition>.BeginScope().WithContext<IScopeContext>(new ScopeContext()));
     * 
     * scope.AddTransient<IDynamicTransientService, DynamicTransientService>();
     * scope.AddSingleton<IDynamicSingletonService, DynamicSingletonService>();
     * var container = scope.Build();
     * var scopedService = container.Get<IScopedService>();
     */
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDefinition">
    /// This is a Tag definition, allowing us to separate application scopes.
    /// An empty class or empty interface would be sufficient
    /// </typeparam>
    public static class Scope<TDefinition> where TDefinition : IScopeDefinition {
        private static IDIBuilder m_Builder;

        internal static void Set(IDIBuilder builder) => m_Builder = builder;
        
        public static IScopeBuilder BeginScope() {
            return new ScopeBuilder(m_Builder);
        }
    }

    internal sealed class StaticServiceSingletonResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> where TAbstract : IService {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;

        private Singleton<TAbstract, TConcrete> m_Singleton;
        
        public void Resolve() {
            m_Singleton = new Singleton<TAbstract, TConcrete>(m_Configuration, m_Factory);
            Service<TAbstract>.Set(m_Singleton);
        }
        
        public void Dispose() {
            if (Service<TAbstract>.Get() is IDisposable disposable) {
                disposable.Dispose();
            }
            
            Service<TAbstract>.Set(null);
            m_Singleton.Dispose(); 
        }
        
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }       
        
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }
    
    internal sealed class StaticControllerSingletonResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> where TAbstract : IController {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;

        private Singleton<TAbstract, TConcrete> m_Singleton; 
        
        public void Resolve() {
            m_Singleton = new Singleton<TAbstract, TConcrete>(m_Configuration, m_Factory);
            Controller<TAbstract>.Set(m_Singleton);
        }
        
        public void Dispose() {
            if (Controller<TAbstract>.Get() is IDisposable disposable) {
                disposable.Dispose();
            }
            
            Controller<TAbstract>.Set(null);
            m_Singleton.Dispose(); 
        }
        
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }       
        
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }
    
    internal sealed class HiddenStaticSingletonResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;
        private Singleton<TAbstract,TConcrete> m_Singleton;

        public void Resolve() {
            m_Singleton = new Singleton<TAbstract, TConcrete>(m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Singleton.Resolve() is IDisposable disposable) {
                disposable.Dispose();
            }

            m_Singleton.Dispose();
            m_Singleton = null;
        }
        
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }       
        
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }

    internal sealed class StaticRepositoryResolver<TAbstract, TConcrete> : IResolver, IConfigurable<TConcrete> {
        private Func<TConcrete, TConcrete> m_Configuration = ConfigurationFactory.Empty<TConcrete>();
        private Func<TConcrete> m_Factory;
        private Singleton<TAbstract,TConcrete> m_Singleton;

        public void Resolve() {
            m_Singleton = new Singleton<TAbstract, TConcrete>(m_Configuration, m_Factory);
        }

        public void Dispose() {
            if (m_Singleton.Resolve() is IDisposable disposable) {
                disposable.Dispose();
            }
            
            m_Singleton.Dispose();
            m_Singleton = null;
        }
        
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithConfiguration(Func<TConcrete, TConcrete> configuration) {
            m_Configuration = configuration;
            return this;
        }
        
        IConfigurable<TConcrete> IConfigurable<TConcrete>.WithFactory(Func<TConcrete> factory) {
            m_Factory = factory;
            return this;
        }
    }

    public interface IServiceCollection : IResolver {
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IService;
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IService ;
    }
    
    public sealed class ApplicationServiceCollection : IServiceCollection {
        private readonly List<IResolver> m_Resolvers = new();
        
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IService {
            var resolver = new StaticServiceTransientResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IService  {
            var resolver = new StaticServiceSingletonResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public void Resolve() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();
        }

        public void Dispose() {
            foreach(var resolver in m_Resolvers) resolver.Dispose();
        }
    }
    
    public interface IControllerCollection : IResolver {
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IController;
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IController;
    }
    
    public sealed class ApplicationControllerCollection : IControllerCollection {
        private readonly List<IResolver> m_Resolvers = new();
        
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IController {
            var resolver = new StaticControllerTransientResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IController {
            var resolver = new StaticControllerSingletonResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public void Resolve() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();
        }

        public void Dispose() {
            foreach(var resolver in m_Resolvers) resolver.Dispose();
        }
    }
    
    public interface IConfigurationCollection : IResolver {
        public IConfigurable<TConcrete> AddConfiguration<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IConfiguration; 
    }
    
    public sealed class ApplicationConfigurationCollection : IConfigurationCollection {
        private readonly List<IResolver> m_Resolvers = new();
        
        public IConfigurable<TConcrete> AddConfiguration<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IConfiguration {
            var resolver = new StaticConfigurationResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public void Resolve() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();
        }

        public void Dispose() {
            foreach(var resolver in m_Resolvers) resolver.Dispose();
        }
    }
    
    public interface IHiddenCollection : IResolver {
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract; 
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract; 
    }

    public sealed class ApplicationHiddenCollection : IHiddenCollection {
        private readonly List<IResolver> m_Resolvers = new();
        
        public IConfigurable<TConcrete> AddTransient<TAbstract, TConcrete>() where TConcrete : TAbstract {
            var resolver = new HiddenStaticTransientResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public IConfigurable<TConcrete> AddSingleton<TAbstract, TConcrete>() where TConcrete : TAbstract {
            var resolver = new HiddenStaticSingletonResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public void Resolve() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();
        }

        public void Dispose() {
            foreach(var resolver in m_Resolvers) resolver.Dispose();
        }
    }

    public interface IRepositoryCollection : IResolver {
        public IConfigurable<TConcrete> AddRepository<TAbstract, TConcrete>()
            where TConcrete : TAbstract where TAbstract : IRepository;
    }

    public sealed class ApplicationRepositoryCollection : IRepositoryCollection {
        private readonly List<IResolver> m_Resolvers = new();

        public IConfigurable<TConcrete> AddRepository<TAbstract, TConcrete>() where TConcrete : TAbstract where TAbstract : IRepository {
            var resolver = new StaticRepositoryResolver<TAbstract, TConcrete>();
            m_Resolvers.Add(resolver);
            return resolver;
        }

        public void Resolve() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();
        }

        public void Dispose() {
            foreach(var resolver in m_Resolvers) resolver.Dispose();
        }
    }
    
    // builder.Scopes.DefineScope<ScopeDefinition>((builder) => {
    //     builder.Add<IScopedService, ScopedService>()
    //         .WithConfiguration(configuration => { } );
    //         .WithFactory(() => new ScopedService()); // context is not available to scoped services created via a factory.
    // });
    
    internal sealed class ScopeResolver<TScopeDefinition> : IResolver, IConfigurable<IDIBuilder> where TScopeDefinition : IScopeDefinition {
        private Func<IDIBuilder, IDIBuilder> m_Configuration = ConfigurationFactory.Empty<IDIBuilder>();
        private Func<IDIBuilder> m_Factory;
        
        public void Resolve() {
            var builder = m_Factory == null ? new DIBuilder() : m_Factory();
            builder = m_Configuration(builder);
            Scope<TScopeDefinition>.Set(builder);
        }
        
        public void Dispose() { }
        
        IConfigurable<IDIBuilder> IConfigurable<IDIBuilder>.WithConfiguration(Func<IDIBuilder, IDIBuilder> configuration) {
            m_Configuration = configuration;
            return this;
        }
        
        IConfigurable<IDIBuilder> IConfigurable<IDIBuilder>.WithFactory(Func<IDIBuilder> factory) {
            m_Factory = factory;
            return this;
        }
    }
    
    public interface IScopeCollection : IResolver {
        IConfigurable<IDIBuilder> DefineScope<TScopeDefinition>() where TScopeDefinition : IScopeDefinition;
    }

    public sealed class ApplicationScopeCollection : IScopeCollection {
        private readonly List<IResolver> m_Resolvers = new();
        
        public IConfigurable<IDIBuilder> DefineScope<TScopeDefinition>() where TScopeDefinition : IScopeDefinition {
            var resolver = new ScopeResolver<TScopeDefinition>();
            m_Resolvers.Add(resolver);
            return resolver;
        }
        
        public void Resolve() {
            foreach(var resolver in m_Resolvers) resolver.Resolve();
        }
        
        public void Dispose() {
            foreach(var resolver in m_Resolvers) resolver.Dispose();
        }
    }

    public interface IContainerBuilder<TScope> {
        public TScope Build<TType>() where TType : TScope;
    }
    
    public sealed class UnityApplicationCleanup : IDisposable {
        private readonly IDisposable m_Disposable;

        public UnityApplicationCleanup(IDisposable[] Disposables) {
            m_Disposable = new DisposableCollection(Disposables);
        }
        
        public void Dispose() {
            m_Disposable.Dispose();
        }
    }
    
    public sealed class UnityApplicationBuilder : IContainerBuilder<UnityApplication> {
        private readonly IServiceCollection m_Services = new ApplicationServiceCollection();
        private readonly IConfigurationCollection m_Configurations = new ApplicationConfigurationCollection();
        private readonly IControllerCollection m_Controllers = new ApplicationControllerCollection();
        private readonly IHiddenCollection m_Hidden = new ApplicationHiddenCollection();
        private readonly IRepositoryCollection m_Repositories = new ApplicationRepositoryCollection();
        private readonly IScopeCollection m_Scopes = new ApplicationScopeCollection();

        private IDisposable m_Disposable;

        public IServiceCollection Services { get { return m_Services; } }
        public IConfigurationCollection Configurations { get { return m_Configurations; } }
        public IControllerCollection Controllers { get { return m_Controllers; } }
        public IHiddenCollection Hidden { get { return m_Hidden; } }
        public IRepositoryCollection Repositories { get { return m_Repositories; } }
        public IScopeCollection Scopes { get { return m_Scopes; } }
        
        public UnityApplicationCleanup Cleanup {
            set { m_Disposable = value; }
        }
        
        public UnityApplication Build<T>() where T : UnityApplication {
            m_Services.Resolve();
            m_Controllers.Resolve();
            m_Hidden.Resolve();
            m_Repositories.Resolve();
            m_Scopes.Resolve();
            
            var cleanup = m_Disposable ?? new DisposableCollection(new IDisposable[]{ m_Services, Controllers, m_Hidden, m_Repositories });

            return (T)Activator.CreateInstance(typeof(T), new object[] { cleanup });
        }
    }
    
    internal sealed class DisposableCollection : IDisposable {
        private readonly IDisposable[] m_Disposables;
        
        public DisposableCollection(IDisposable[] disposables) {
            m_Disposables = disposables;
        }

        public void Dispose() {
            foreach(var disposable in m_Disposables) disposable.Dispose();
        }
    }

    public interface IUnityApplication {
        Awaitable Run();
        void Quit();
    }

    public abstract class UnityApplication : IUnityApplication {
        protected UnityApplication(IDisposable cleanup) {}
        
        public abstract Awaitable Run();

        public abstract void Quit();
    }
    
    public abstract class UnityApplicationLoaderBase_SO : ScriptableObject {
        public abstract Awaitable LoadAsync(CancellationToken cancellationToken);
        public virtual Awaitable UnloadAsync() { return AwaitableUtility.CompletedTask; }
    }
    
    public sealed class UnityApplicationBoot : MonoBehaviour {
        [SerializeField] private UnityApplicationLoaderBase_SO[] m_ApplicationLoaders;

        private IUnityApplication m_Application;
        
        private void Awake() {
            DontDestroyOnLoad(gameObject);
            m_Application = Program.Main();
        }

        private async void Start() {
            await m_Application.Run();
            
            foreach (var loader in m_ApplicationLoaders) {
                await loader.LoadAsync(destroyCancellationToken);
            }
        }

        private async void OnDestroy() {
            foreach (var loader in m_ApplicationLoaders) {
                await loader.UnloadAsync();
            }
            
            m_Application.Quit();
        }
    }
    
    public static class CMDArgs {
            private static IDictionary<string, string> s_Args;
            
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            private static void Main() {
                s_Args = new Dictionary<string, string>(
                    Environment
                    .GetCommandLineArgs()
                    .Read()
                );
            }
    
            public static IDictionary<string, string> Read() {
                return s_Args;
            }
    
            public static bool Exists(string arg) {
                return s_Args.ContainsKey(arg);
            }
            
            public static bool TryGetValue(string key, out string value) {
                value = !s_Args.TryGetValue(key, out var arg) ? default : arg;
                return s_Args.ContainsKey(key);
            }
            
            private static IEnumerable<KeyValuePair<string, string>> Read(this IReadOnlyList<string> args) {
                for (var i = 0; i < args.Count; i++) {
                    var key = args[i];
                    var value = ++i > args.Count - 1 ? string.Empty : args[i];
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }

    public static class Program {
        public static UnityApplication Main() => Main(CMDArgs.Read());

        private static UnityApplication Main(IDictionary<string, string> args) {
            var builder = new UnityApplicationBuilder();

            return builder.Build<UnityApplication>();
        }
    }

    public sealed class UnityConfigurationCollection : ScriptableObject, IResolver {
        [SerializeField] private List<ScriptableObject> m_Configurations;
        
        private readonly ApplicationConfigurationCollection m_Collection = new();
        private bool m_Resolved;
        
        public void Resolve() {
            if (m_Resolved) return;
            
            foreach (IConfiguration config in m_Configurations) {
                if (config == null) throw new ArgumentNullException(nameof(config), $"All ScriptableObjects listed in a {typeof(UnityConfigurationCollection)} must be of type IConfiguration");
                m_Collection.GenerateConstructorFor(config)(m_Collection, config);
            }
            
            m_Collection.Resolve();
            m_Resolved = true;
        }

        public void Dispose() {
            m_Collection.Dispose();
            m_Resolved = false;
        }
    }

    [Serializable]
    internal sealed class UnityObjectResolver {
        [field: SerializeField] public ResolverType Type { get; private set; }
        [field: SerializeField] public ScriptableObject Object { get; private set; }
    }
    
    public sealed class UnityStaticServiceCollection : ScriptableObject, IResolver {
        [SerializeField] private List<UnityObjectResolver> m_Services;
        
        private readonly ApplicationServiceCollection m_Collection = new();
        private bool m_Resolved;
        
        public void Resolve() {
            if (m_Resolved) return;
            
            foreach (var resolver in m_Services) {
                if(resolver.Object is not IService service) throw new ArgumentNullException(nameof(resolver.Object), $"All ScriptableObjects listed in a {typeof(UnityStaticServiceCollection)} must be of type IService");
                m_Collection.GenerateConstructorFor(service, resolver.Type)(m_Collection, service);
            }
            
            m_Collection.Resolve();
            m_Resolved = true;
        }

        public void Dispose() {
            m_Collection.Dispose();
            m_Resolved = false;
        }
    }
    
    public sealed class UnityStaticControllerCollection : ScriptableObject, IResolver {
        [SerializeField] private List<UnityObjectResolver> m_Controllers;
        
        private readonly ApplicationControllerCollection m_Collection = new();
        private bool m_Resolved;
        
        public void Resolve() {
            if (m_Resolved) return;
            
            foreach (var resolver in m_Controllers) {
                if(resolver.Object is not IController controller) 
                    throw new ArgumentNullException(nameof(resolver.Object), $"All ScriptableObjects listed in a {typeof(UnityStaticControllerCollection)} must be of type IController or IService or IConfiguration or IRepository");
    
                m_Collection.GenerateConstructorFor(controller, resolver.Type)(m_Collection, controller);
            }
            
            m_Collection.Resolve();
            m_Resolved = true;
        }

        public void Dispose() {
            m_Collection.Dispose();
            m_Resolved = false;
        }
    }
    
    public sealed class UnityStaticHiddenCollection : ScriptableObject, IResolver {
        [SerializeField] private List<UnityObjectResolver> m_Hidden;
        
        private readonly ApplicationHiddenCollection m_Collection = new();
        private bool m_Resolved;
        
        public void Resolve() {
            if (m_Resolved) return;
            
            foreach (var resolver in m_Hidden) {
                if(resolver.Object is not (IController or IService or IConfiguration or IRepository))
                    throw new ArgumentNullException(nameof(resolver.Object), $"All ScriptableObjects listed in a {typeof(UnityStaticHiddenCollection)} must be of type IHidden");
                m_Collection.GenerateConstructorFor(resolver.Object, resolver.Type)(m_Collection, resolver.Object);
            }
            
            m_Collection.Resolve();
            m_Resolved = true;
        }

        public void Dispose() {
            m_Collection.Dispose();
            m_Resolved = false;
        }
    }
    
    public sealed class UnityStaticRepositoryCollection : ScriptableObject, IResolver {
        [SerializeField] private List<UnityObjectResolver> m_Repositories;
        
        private readonly ApplicationRepositoryCollection m_Collection = new();
        private bool m_Resolved;
        
        public void Resolve() {
            if (m_Resolved) return;
            
            foreach (var resolver in m_Repositories) {
                if(resolver.Object is not IRepository repository)
                    throw new ArgumentNullException(nameof(resolver.Object), $"All ScriptableObjects listed in a {typeof(UnityStaticRepositoryCollection)} must be of type IRepository");
                m_Collection.GenerateConstructorFor(repository)(m_Collection, repository);
            }
            
            m_Collection.Resolve();
            m_Resolved = true;
        }

        public void Dispose() {
            m_Collection.Dispose();
            m_Resolved = false;
        }
    }

    internal delegate void ConfigRegistration(IConfigurationCollection container, IConfiguration config);
    internal delegate void ServiceRegistration(IServiceCollection container, IService service);
    internal delegate void ControllerRegistration(IControllerCollection container, IController controller);
    internal delegate void HiddenRegistration(IHiddenCollection container, object resolver);
    internal delegate void RepositoryRegistration(IRepositoryCollection container, IRepository controller);
    internal delegate void ContainerRegistration(IDIBuilder builder, object resolver);

    public enum ResolverType : byte {
        Singleton,
        Transient,
    }
    
    internal static class UnityResolverConstructorUtility {
        public static ConfigRegistration GenerateConstructorFor(this IConfigurationCollection container, IConfiguration config) {
            var concreteType = config.GetType();

            var abstractType = concreteType
                .GetInterfaces()
                .FirstOrDefault(i => typeof(IConfiguration).IsAssignableFrom(i) && i != typeof(IConfiguration));

            if (abstractType == null) {
                throw new ObjectResolverException($"No abstract IConfiguration-compatible interface found on {concreteType.Name}");
            }

            var containerParam = Expression.Parameter(typeof(object), "container");
            var configParam = Expression.Parameter(typeof(IConfiguration), "config");

            var containerCast = Expression.Convert(containerParam, container.GetType());

            var addConfigMethod = container.GetType()
                             .GetMethod("AddConfiguration", BindingFlags.Public | BindingFlags.Instance)
                             ?.MakeGenericMethod(abstractType, concreteType)
                         ?? throw new ObjectResolverException("AddConfiguration<TA, TC> method not found :: THIS SHOULD NOT HAPPEN"); 

            var configConfigurable = Expression.Call(containerCast, addConfigMethod);

            var withFactoryMethod = configConfigurable.Type.GetMethod("WithFactory")
                                    ?? throw new ObjectResolverException("WithFactory method not found");

            var lambdaBody = Expression.Convert(configParam, concreteType);
            var factoryLambda = Expression.Lambda(lambdaBody);

            var funcType = typeof(Func<>).MakeGenericType(concreteType);
            var factoryConverted = Expression.Convert(factoryLambda, funcType);

            var callWithFactory = Expression.Call(configConfigurable, withFactoryMethod, factoryConverted);

            return Expression.Lambda<ConfigRegistration>(callWithFactory, containerParam, configParam).Compile();
        }
        public static ServiceRegistration GenerateConstructorFor(this IServiceCollection container, IService service, ResolverType type = ResolverType.Singleton) {
            var concreteType = service.GetType();

            var abstractType = concreteType
                .GetInterfaces()
                .FirstOrDefault(i => typeof(IService).IsAssignableFrom(i) && i != typeof(IService));

            if (abstractType == null) {
                throw new ObjectResolverException($"No abstract IService-compatible interface found on {concreteType.Name}");
            }

            var containerParam = Expression.Parameter(typeof(object), "container");
            var serviceParam = Expression.Parameter(typeof(IService), "service");

            var containerCast = Expression.Convert(containerParam, container.GetType());

            var methodName = type switch {
                ResolverType.Singleton => "AddSingleton",
                ResolverType.Transient => "AddTransient",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            var addMethod = container.GetType()
                             .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                             ?.MakeGenericMethod(abstractType, concreteType)
                         ?? throw new ObjectResolverException("Appropriate method not found :: THIS SHOULD NOT HAPPEN"); 

            var configurable = Expression.Call(containerCast, addMethod);

            var withFactoryMethod = configurable.Type.GetMethod("WithFactory")
                                    ?? throw new ObjectResolverException("WithFactory method not found");

            var lambdaBody = Expression.Convert(serviceParam, concreteType);
            var factoryLambda = Expression.Lambda(lambdaBody);

            var funcType = typeof(Func<>).MakeGenericType(concreteType);
            var factoryConverted = Expression.Convert(factoryLambda, funcType);

            var callWithFactory = Expression.Call(configurable, withFactoryMethod, factoryConverted);

            return Expression.Lambda<ServiceRegistration>(callWithFactory, containerParam, serviceParam).Compile();
        }
        public static ControllerRegistration GenerateConstructorFor(this IControllerCollection container, IController controller, ResolverType type = ResolverType.Singleton) {
            var concreteType = controller.GetType();

            var abstractType = concreteType
                .GetInterfaces()
                .FirstOrDefault(i => typeof(IController).IsAssignableFrom(i) && i != typeof(IController));

            if (abstractType == null) {
                throw new ObjectResolverException($"No abstract IController-compatible interface found on {concreteType.Name}");
            }

            var containerParam = Expression.Parameter(typeof(object), "container");
            var controllerParam = Expression.Parameter(typeof(IController), "controller");

            var containerCast = Expression.Convert(containerParam, container.GetType());

            var methodName = type switch {
                ResolverType.Singleton => "AddSingleton",
                ResolverType.Transient => "AddTransient",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var addMethod = container.GetType()
                                .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                                ?.MakeGenericMethod(abstractType, concreteType)
                            ?? throw new ObjectResolverException("Appropriate method not found :: THIS SHOULD NOT HAPPEN");

            var configurable = Expression.Call(containerCast, addMethod);

            var withFactoryMethod = configurable.Type.GetMethod("WithFactory")
                                    ?? throw new ObjectResolverException("WithFactory method not found");

            var lambdaBody = Expression.Convert(controllerParam, concreteType);
            var factoryLambda = Expression.Lambda(lambdaBody);

            var funcType = typeof(Func<>).MakeGenericType(concreteType);
            var factoryConverted = Expression.Convert(factoryLambda, funcType);

            var callWithFactory = Expression.Call(configurable, withFactoryMethod, factoryConverted);

            return Expression.Lambda<ControllerRegistration>(callWithFactory, containerParam, controllerParam).Compile();
        }
        public static HiddenRegistration GenerateConstructorFor(this IHiddenCollection container, object resolver, ResolverType type = ResolverType.Singleton) {
            var concreteType = resolver.GetType();

            var interfaces = concreteType.GetInterfaces();

            var abstractType = interfaces.FirstOrDefault(i => typeof(IService).IsAssignableFrom(i) && i != typeof(IService)) ??
                               interfaces.FirstOrDefault(i => typeof(IController).IsAssignableFrom(i) && i != typeof(IController)) ??
                               interfaces.FirstOrDefault(i => typeof(IConfiguration).IsAssignableFrom(i) && i != typeof(IConfiguration)) ??
                               interfaces.FirstOrDefault(i => typeof(IRepository).IsAssignableFrom(i) && i != typeof(IRepository));
            
            if (abstractType == null) {
                throw new ObjectResolverException($"No abstract compatible interface found on {concreteType.Name} :: Compatible with " +
                                                  $"IService, IController, IRepository, IConfiguration");
            }

            var containerParam = Expression.Parameter(typeof(object), "container");
            var resolverParam = Expression.Parameter(typeof(IController), "resolver");

            var containerCast = Expression.Convert(containerParam, container.GetType());

            var methodName = type switch {
                ResolverType.Singleton => "AddSingleton",
                ResolverType.Transient => "AddTransient",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var addMethod = container.GetType()
                                .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                                ?.MakeGenericMethod(abstractType, concreteType)
                            ?? throw new ObjectResolverException("Appropriate method not found :: THIS SHOULD NOT HAPPEN");

            var configurable = Expression.Call(containerCast, addMethod);

            var withFactoryMethod = configurable.Type.GetMethod("WithFactory")
                                    ?? throw new ObjectResolverException("WithFactory method not found");

            var lambdaBody = Expression.Convert(resolverParam, concreteType);
            var factoryLambda = Expression.Lambda(lambdaBody);

            var funcType = typeof(Func<>).MakeGenericType(concreteType);
            var factoryConverted = Expression.Convert(factoryLambda, funcType);

            var callWithFactory = Expression.Call(configurable, withFactoryMethod, factoryConverted);

            return Expression.Lambda<HiddenRegistration>(callWithFactory, containerParam, resolverParam).Compile();
        }
        public static RepositoryRegistration GenerateConstructorFor(this IRepositoryCollection container, IRepository repository) {
            var concreteType = repository.GetType();

            var abstractType = concreteType
                .GetInterfaces()
                .FirstOrDefault(i => typeof(IRepository).IsAssignableFrom(i) && i != typeof(IRepository));
            
            if (abstractType == null) {
                throw new ObjectResolverException($"No abstract compatible interface found on {concreteType.Name} :: Compatible with " +
                                                  $"IRepository");
            }

            var containerParam = Expression.Parameter(typeof(object), "container");
            var repositoryParam = Expression.Parameter(typeof(IController), "repository");

            var containerCast = Expression.Convert(containerParam, container.GetType());

            var addMethod = container.GetType()
                                .GetMethod("AddRepository", BindingFlags.Public | BindingFlags.Instance)
                                ?.MakeGenericMethod(abstractType, concreteType)
                            ?? throw new ObjectResolverException("Appropriate method not found :: THIS SHOULD NOT HAPPEN");

            var configurable = Expression.Call(containerCast, addMethod);

            var withFactoryMethod = configurable.Type.GetMethod("WithFactory")
                                    ?? throw new ObjectResolverException("WithFactory method not found");

            var lambdaBody = Expression.Convert(repositoryParam, concreteType);
            var factoryLambda = Expression.Lambda(lambdaBody);

            var funcType = typeof(Func<>).MakeGenericType(concreteType);
            var factoryConverted = Expression.Convert(factoryLambda, funcType);

            var callWithFactory = Expression.Call(configurable, withFactoryMethod, factoryConverted);

            return Expression.Lambda<RepositoryRegistration>(callWithFactory, containerParam, repositoryParam).Compile();
        }
        public static ContainerRegistration GenerateConstructorFor(this IDIBuilder builder, object resolver, ResolverType type = ResolverType.Singleton) {
            var concreteType = resolver.GetType();

            var interfaces = concreteType.GetInterfaces();

            var abstractType = interfaces.FirstOrDefault(i => typeof(IService).IsAssignableFrom(i) && i != typeof(IService)) ??
                               interfaces.FirstOrDefault(i => typeof(IController).IsAssignableFrom(i) && i != typeof(IController)) ??
                               interfaces.FirstOrDefault(i => typeof(IConfiguration).IsAssignableFrom(i) && i != typeof(IConfiguration)) ??
                               interfaces.FirstOrDefault(i => typeof(IRepository).IsAssignableFrom(i) && i != typeof(IRepository));
            
            if (abstractType == null) {
                throw new ObjectResolverException($"No abstract compatible interface found on {concreteType.Name} :: Compatible with " +
                                                  $"IService, IController, IRepository, IConfiguration");
            }

            var containerParam = Expression.Parameter(typeof(object), "container");
            var resolverParam = Expression.Parameter(typeof(IController), "resolver");

            var containerCast = Expression.Convert(containerParam, builder.GetType());

            var methodName = type switch {
                ResolverType.Singleton => "AddSingleton",
                ResolverType.Transient => "AddTransient",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var addMethod = builder.GetType()
                                .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance)
                                ?.MakeGenericMethod(abstractType, concreteType)
                            ?? throw new ObjectResolverException("Appropriate method not found :: THIS SHOULD NOT HAPPEN");

            var configurable = Expression.Call(containerCast, addMethod);

            var withFactoryMethod = configurable.Type.GetMethod("WithFactory")
                                    ?? throw new ObjectResolverException("WithFactory method not found");

            var lambdaBody = Expression.Convert(resolverParam, concreteType);
            var factoryLambda = Expression.Lambda(lambdaBody);

            var funcType = typeof(Func<>).MakeGenericType(concreteType);
            var factoryConverted = Expression.Convert(factoryLambda, funcType);

            var callWithFactory = Expression.Call(configurable, withFactoryMethod, factoryConverted);

            return Expression.Lambda<ContainerRegistration>(callWithFactory, containerParam, resolverParam).Compile();
        }
        
        public static ContainerDefinition GenerateContainerConstructor(this ApplicationContainer container, Type definitionType) {
            var methodInfo = container.GetType()
                .GetMethod("DefineContainer", BindingFlags.Instance | BindingFlags.Public)!
                .MakeGenericMethod(definitionType);

            var containerParam = Expression.Parameter(typeof(ApplicationContainer), "container");
            var callExpr = Expression.Call(containerParam, methodInfo);
            return Expression.Lambda<ContainerDefinition>(callExpr, containerParam).Compile();
        }
    }
    public sealed class StaticUnityApplicationScope : ScriptableObject, IResolver {
        [SerializeField] private UnityConfigurationCollection m_ConfigurationCollection; 
        [SerializeField] private UnityStaticServiceCollection m_ServiceCollection; 
        [SerializeField] private UnityStaticControllerCollection m_ControllerCollection; 
        [SerializeField] private UnityStaticHiddenCollection m_HiddenCollection; 
        [SerializeField] private UnityStaticRepositoryCollection m_RepositoryCollection;
        
        public void Resolve() {
            if( m_ConfigurationCollection != null ) { m_ConfigurationCollection.Resolve(); }
            if( m_ServiceCollection != null ) { m_ServiceCollection.Resolve(); }
            if( m_ControllerCollection != null ) { m_ControllerCollection.Resolve(); }
            if( m_HiddenCollection != null ) { m_HiddenCollection.Resolve(); }
            if( m_RepositoryCollection != null ) { m_RepositoryCollection.Resolve(); }
        }
        
        public void Dispose() {
            if( m_ConfigurationCollection != null ) { m_ConfigurationCollection.Dispose(); }
            if( m_ServiceCollection != null ) { m_ServiceCollection.Dispose(); }
            if( m_ControllerCollection != null ) { m_ControllerCollection.Dispose(); }
            if( m_HiddenCollection != null ) { m_HiddenCollection.Dispose(); }
            if( m_RepositoryCollection != null ) { m_RepositoryCollection.Dispose(); }
        }
    }

    public sealed class UnityApplicationScope : ScriptableObject, IResolver {
        [SerializeField] private List<UnityObjectResolver> m_Resolvers;
        
        private readonly IDIBuilder m_DIBuilder = new DIBuilder();
        
        private IDIContainer m_Container;

        public IDIContainer Container => m_Container;
        
        public void Resolve() {
            foreach (var resolver in m_Resolvers) {
                m_DIBuilder.GenerateConstructorFor(resolver.Object, resolver.Type)(m_DIBuilder, resolver.Object);
            }

            m_Container = m_DIBuilder.Build();
        }
        
        public void Dispose() {
            m_Container?.Dispose();
        }
    }
}