using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Autofac;
using Autofac.Integration.Mvc;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

using DancingGoat.Infrastructure;
using DancingGoat.Repositories;
using DancingGoat.Services;

namespace DancingGoat
{
    /// <summary>
    /// Registers required implementations to the Autofac container and set the container as ASP.NET MVC dependency resolver
    /// </summary>
    public static class DependencyResolverConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();

            ConfigureDependencyResolverForMvcApplication(builder);

            AttachCMSDependencyResolver(builder);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }


        private static void ConfigureDependencyResolverForMvcApplication(ContainerBuilder builder)
        {
            // Enable property injection in view pages
            builder.RegisterSource(new ViewRegistrationSource());

            // Register web abstraction classes
            builder.RegisterModule<AutofacWebTypesModule>();

            // Register controllers
            builder.RegisterControllers(typeof(DancingGoatApplication).Assembly);

            // Register repositories
            builder.RegisterAssemblyTypes(typeof(DancingGoatApplication).Assembly)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IRepository).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            // Register services
            builder.RegisterAssemblyTypes(typeof(DancingGoatApplication).Assembly)
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IService).IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            // Register providers of additional information about content items
            builder.RegisterType<ContentItemMetadataProvider>()
                .AsImplementedInterfaces()
                .SingleInstance();

            // Register factory for product view models
            builder.RegisterType<TypedProductViewModelFactory>()
                .SingleInstance();

            // Register factory for full-text search product view models
            builder.RegisterType<TypedSearchItemViewModelFactory>()
                .InstancePerRequest();

            // Enable declaration of output cache dependencies in controllers
            builder.Register(context => new OutputCacheDependencies(context.Resolve<HttpResponseBase>(), context.Resolve<IContentItemMetadataProvider>(), IsCacheEnabled()))
                .AsImplementedInterfaces()
                .InstancePerRequest();
        }


        /// <summary>
        /// Configures Autofac container to use CMS dependency resolver in case it cannot resolve a dependency.
        /// </summary>
        private static void AttachCMSDependencyResolver(ContainerBuilder builder)
        {
            builder.RegisterSource(new CMSRegistrationSource());
        }


        private static bool IsCacheEnabled()
        {
            return !IsPreviewEnabled();
        }


        private static bool IsPreviewEnabled()
        {
            return HttpContext.Current.Kentico().Preview().Enabled;
        }
    }
}