# BUTTR - The Unity Dependency Injection Framework
> Built for Unity 6 onwards.

Using Expression Trees at it's core and minimising reflection by using code generation for attribute injection. Buttr is
designed to be lightning fast at resolving dependencies whenever you need them and is extendable to allow for users to add
re-usable configurable packages.

- `[Inject]` & `[InjectScope(key)]` Attributes allow for MonoBehaviour injection of application dependencies and scoped dependencies
- Build Application wide dependencies using a ` new ApplicationBuilder()`
- Use `Application<T>.Get()` to access application wide dependencies anywhere
- Build scope containers using a `new ScopeBuilder(string key)` and access them directly `ScopeRegistry.Get(key)`
- resolve dependencies anywhere using `new DIBuilder()`

## Installation
You can install Buttr through the Unity Package Manager Or via unity package through the Releases page;

### Unity Package Manager
1. Open up Unity
2. Click the lil' '+' at the top left
3. 'Install package from git url'
4. Paste `https://github.com/FriedCrumpets/Buttr.git?path=Assets/Plugins/Buttr`

### Releases
> [ReleaseLink](https://github.com/FriedCrumpets/Buttr/releases/tag/v1.0.0)

## Getting Started
### Application
ApplicationBuilders don't create a container object. They resolve to the static resolver registry and are accessed through
`Application<T>.Get();`. The static registry is used for all Dependency Injection containers throughout the application.
It's recommended to use one `ApplicationBuilder` per Unity Application. Use this to create the your applications main framework.
```csharp
// Create Application Wide containers using an Application Builder
var builder = new ApplicationBuilder();

// Add objects to this container, either singleton or transient
// Singletons only ever resolve to a single instance
builder.Resolvers.AddSingleton<ISingletonFoo, SingletonFoo>(); // <Abstract, Concrete>
builder.Resolvers.AddSingleton<SingletonBar>(); // <Concrete>

// Transients resolve for a new instance each time they are injected or pulled from the container
builder.Resolvers.AddTransient<ITransientFoo, TransientFoo>(); // <Abstract, Concrete>
builder.Resolvers.AddTransient<TransientBar>(); // <Concrete>

// you can also hide objects to be available for injection, but not through Application<T>.Get();
builder.Hidden.AddSingleton<ISingletonHidden, SingletonHidden>();
builder.Hidden.AddTransient<SingletonHiddenBar>();

// Adding an object to the builder returns a IConfigurable<TConcrete>
// you can override the factory or force a specific configuration into the object
builder.Resolvers.AddTransient<TestConfigurable>()
    .WithFactory(() => return new TestConfigurable())
    .WithConfiguration((testConfigurable) => { testConfigurable.Foo = "Bar"; return testConfigurable; });

// building the application container returns an IDisposable LifeTime object
var app = builder.Build();

// After building we can access our objects via Application<T>.Get();
// Due to lazy instantiation objects will only be allocated when requested
var foo = Application.Get<ITransientFoo>(); 
var bar = Application.Get<SingletonBar>();

// Dispose of the lifetime to remove all from the static resolver registry and clear the Application<T>'s
// disposing also disposes of all resolved singletons, transients are not disposed
app.Dispose();
var fooThrows = Application.Get<ITransientFoo>(); // This will throw a NullReferenceException
```

### Scope
Scopes are created via a `new ScopeBuilder(string key);`. Upon building a scope it will register itself with the static
scope registry to be injected into MonoBehaviours or accessed via `ScopeRegistry.Get(key);`
```csharp
var builder = new ScopeBuilder("key");

// Add singletons and transients the same way as an application container
builder.AddTransient<IFoo, Foo>();
builder.AddSingleton<Bar>();

// If you want an object to be available for constructor injection 
// but not gettable from the container it must inherit from IHidden
sealed class HiddenTransient : IHidden { } 
builder.AddTransient<HiddenTransient>();

// Adding an object to the builder returns a IConfigurable<TConcrete>
// you can override the factory or force a specific configuration into the object
builder.AddTransient<TestConfigurable>()
    .WithFactory(() => return new TestConfigurable())
    .WithConfiguration((testConfigurable) => { testConfigurable.Foo = "Bar"; return testConfigurable; });

// building registers the container with ScopeRegistry
// building first checks the container for dependencies and then checks the application wide dependencies
// meaning you can overwrite application behaviours for scopes if necessary
var container = builder.Build();

// Get objects directly from the container
var foo = container.Get<IFoo>();

// trying to get a hidden object will throow
var hidden = container.Get<HiddenTransient>(); // throws ObjectResolverException

// you can get the scope container elsewhere via
var sameContainer = ScopeRegistry.Get("key");

// disposing of the container gets rid of the container in the scopeRegistry and disposes
container.Dispose();

// this will dispose of the container for all users of the container
sameContainer.Get<IFoo>(); // will throw
```

### DIBuilder
If you want to resolve objects anywhere in the project use a DIBuilder.
`var builder = new DIBuilder();`. as you expect has similar functionality as a ScopeBuilder and ScopeContainer except
nothing is registered to be statically accessible. Will resolve for dependencies registered with the container first and if
they're not present it will look to the static application container for the rest.

### Configurables
When developing to add a full package of functionality to a container there is an object called `ConfigurableCollection`
```csharp
public static IConfigurableCollection UseMyPackage(this ApplicationBuilder builder) { 
    return new ConfigurableCollection()
        .Register(builder.Resolvers.AddSingleton<MyFoo>())
        .Register(builder.Resolvers.AddSingleton<IMyBar, MyBar>())
}

var builder = new ApplicationBuilder();
builder.UseMyPackage()
    .WithFactory<MyFoo>(() => return new MyFoo())
    .WithConfiguration<MyBar>((bar) => { bar.SomeProperty = "bar"; return bar; }
   
builder.Build();
```
ConfigurableCollections can be used with all builders
- ApplicationBuilder
- ScopeBuilder
- DIBuilder

Letting you port in packages from other developers or developing a package yourself. The idea is to simplify adding packages
with a single line while providing the users with a way to configure said Package to suit their needs.

### MonoBehaviour Injection
Originally this framework was built to not use MonoBehaviour injection. The idea was to get your dependencies manually
in `Awake` using `Application.Get<T>()` however someone twisted my arm. The reason for this.

> I don't like runtime reflection

Which is why injection uses code generation and static injection, but it does come with a caveat.
```csharp
// when wanting to inject into a MonoBehaviour we need to make that MonoBehaviour partial
public partial class MyMonoBehaviour : MonoBehaviour { 
    [Inject] private IFoo _Foo;
    [InjectScope("scope")] private IFoo _scopeFoo; 
}
```
And that's it. Buttr will generate the injection code and slap it in a folder in your project.
By default this folder is `Assets/Buttr/Injection/` but you can modify the configuration file located in
`Assets/Buttr` to change this at any time.

The `InjectionConfiguration` Scriptable Object will be generated automatically the first time you use the `[Inject]` attribute.
Once created this is the base of operations for managing your injected code.

Right click on the ScriptableObject in the inspector to either `Clear the object cache` or `reset to defaults`.
You will want to clear the cache if you delete or modify a generated injection file.

### Unity Loaders
Loaders provide a really clear and clean way to boot an application. They can be used for anything and are lightweight
easy to configure.

- Create a new Scene, name it boot, main, program, whatever floats your boat
- Create a gameobject in that scene, again name it appropriately
- Add a `UnityApplicationBoot` component to that gameObject.

The `UnityApplicationBoot` Provides a simple async/await way to load `Loaders`

Loaders are custom scriptableObjects that are designed to work like blocks of loading logic.
```csharp
public sealed class ApplicationLoader : UnityApplicationLoaderBase { 
    private ApplicationLifetime m_Lifetime;
    
    public override Awaitable LoadAsync(CancellationToken cancellationToken) { 
        // cancellationToken is the UnityApplicationBoot gameObject lifetime cancellationToken
        var builder = new ApplicationBuilder();
        
        // add resolvers
        
        m_LifeTime = builder.Build();
        
        // because we haven't awaited anything here we can return with 
        return AwaitableUtility.CompletedTask;
    }
    
    public override Awaitable UnloadAsync() {
        m_LifeTime?.Dispose();    
        return AwaitableUtility.CompletedTask;
    }
}
```
In this example we haven't really utilised Awaitable, but you may come across circumstances where this is essential.
- Pre-warming assets
- Loading scenes
- Web API calls



