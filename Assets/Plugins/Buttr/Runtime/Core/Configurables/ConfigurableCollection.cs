using System;
using System.Collections.Generic;

namespace Buttr.Core {
    /// <summary>
    /// A Collection of Configurables stored and retrieved via Type 
    /// </summary>
    /// <remarks>
    /// This is to be used when creating an application package. In implementing and returning this from a .UsePackage statement
    /// it allows the user to configure the package objects to their needs. Consider this when creating packages for other users 
    /// </remarks>
    /// <example>
    /// public static IConfigurableCollection UsePackage(this UnityApplicationBuilder builder)  {
    ///     return new ConfigurableCollection()
    ///         .Register(builder.Services.AddSingleton{IPackageService, PackageService})
    ///         .Register(builder.Controllers.AddSingleton{IPackageController, PackageController});
    /// }
    ///
    /// var builder = new UnityApplicationBuilder();
    /// builder.UsePackage()
    ///     .WithConfiguration{PackageService}(service => {
    ///         service.SettingA = 1;
    ///         return service;
    ///     }).WithFactory{PackagaeController}(() => return new PackageController(new PackageData(1, 2, 3));
    /// </example>
    /// <remarks>
    /// When allowing for customisation of packages like this refrain from adding objects to the <see cref="ConfigurableCollection"/>
    /// that you do not want users to be able to customise. 
    /// </remarks>
    public sealed class ConfigurableCollection : IConfigurableCollection {
        private readonly Dictionary<Type, object> m_Configurables = new();
        
        public ConfigurableCollection Register<TConcrete>(IConfigurable<TConcrete> configurable) {
            m_Configurables.Add(typeof(TConcrete), configurable);
            return this;
        }
        
        public IConfigurableCollection WithConfiguration<TConcrete>(Func<TConcrete, TConcrete> configuration) {
            if (m_Configurables.TryGetValue(typeof(TConcrete), out var configurable)) {
                var typedConfigurable = (IConfigurable<TConcrete>)configurable;
                typedConfigurable.WithConfiguration(configuration);
            } else 
                throw new ConfigurableException("Configuration for type {typeof(TConcrete)} does not exist within this Configurable Collection");
            
            return this;
        }

        public IConfigurableCollection WithFactory<TConcrete>(Func<TConcrete> factory) {
            if (m_Configurables.TryGetValue(typeof(TConcrete), out var configurable)) {
                var typedConfigurable = (IConfigurable<TConcrete>)configurable;
                typedConfigurable.WithFactory(factory);
            } else 
                throw new ConfigurableException("Configuration for type {typeof(TConcrete)} does not exist within this Configurable Collection");

            return this;
        }
    }
}