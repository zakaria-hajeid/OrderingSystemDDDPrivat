using System.Reflection;

namespace OrderingSystemDDD.Configration
{
    public static class DependencyInjection
    {
        public static IServiceCollection InstallServices(this IServiceCollection service, IConfiguration configuration, params Assembly[] assemblies)
        {
            IEnumerable<IServiceInstaller> serviceInstallers = assemblies.SelectMany(a => a.DefinedTypes).Distinct().Where
                                                                (IsAssignableToType<IServiceInstaller>).Select(Activator.CreateInstance).
                                                                Cast<IServiceInstaller>();


            foreach (IServiceInstaller serviceInstaller in serviceInstallers)
            {
                serviceInstaller.Instal(service, configuration);
            }
            return service;

             static bool IsAssignableToType<T>(TypeInfo typeInfo) =>
             typeof(T).IsAssignableFrom(typeInfo) && !typeInfo.IsInterface && !typeInfo.IsAbstract;
        }

    }
}
