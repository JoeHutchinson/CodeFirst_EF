using System.Web.Http;
using CodeFirst_EF.Collectors;
using CodeFirst_EF.Repositories;
using CodeFirst_EF.Security;
using Unity;
using Unity.Lifetime;

namespace CountVonCount
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();

            // Register context, unit of work and repos with per request lifetime

            // This is a bit more complex than what it needs to be. With one or two factory implementations
            // I can more than half this list, just ran out of time.
            container.RegisterType<ICollector, HtmlCollector>();
            container.RegisterType<IHtmlProvider, HtmlProvider>();
            container.RegisterType<IWordRepository, WordsRepository>();
            container.RegisterType<ISaltCache, WordSaltCache>(new SingletonLifetimeManager());
            container.RegisterType<IHashRepository, HashRepository>();
            container.RegisterType<IHashProvider, PBKDF2Provider>();

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
