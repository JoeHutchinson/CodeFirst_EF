using System.Web.Http;
using CodeFirst_EF.Repositories;
using CodeFirst_EF.Security;
using Unity;
using Unity.Lifetime;

namespace CountVonCount.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();

            // Register context, unit of work and repos with per request lifetime
            container.RegisterType<IWordRepository, WordsRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ISaltCache, WordSaltCache>(new SingletonLifetimeManager());

            // Set Web API dependency resolution to use TinyIoC
            config.DependencyResolver = new UnityResolver(container);
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
