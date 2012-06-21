using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Verde;
using Verde.Executor;
using MvcMusicStore.Models;
using MvcMusicStore.Controllers;

namespace MvcMusicStore
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{*alljs}", new { alljs = @".*\.js" });

            //routes.IgnoreRoute("{*pathInfo}/*.css")

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            var settings = new Verde.Settings
            {
                TestsAssembly = System.Reflection.Assembly.GetExecutingAssembly(),
                AuthorizationCheck = (context) =>
                {
                    // Here we could do something that verifies that the current user is allowed to 
                    // invoke integration tests.  Maybe something like:
                    // return context.User.IsInRole("admin");
                    return true;
                }
            };

            // Because we are using Autofac for IoC, we need this step to ensure that HttpContextWrapper gets 
            // resolved to the Verde.Executor.HttpContextProxy rather than HttpContext.Current. This step is only 
            // necessary if the AutofacDependencyResolver is in use.
            settings.BeginExecuteTestsRequest += (sender, e) =>
            {
                var resolver = DependencyResolver.Current as AutofacDependencyResolver;
                if (resolver != null)
                    new AutofacHttpContextModule().Configure(resolver.ApplicationContainer.ComponentRegistry);
            };

            // Conditionally invoke this line only if integration tests should be enabled in the current environment.
            Verde.Setup.Initialize(settings);

            System.Data.Entity.Database.SetInitializer(new MvcMusicStore.Models.SampleData());

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            AutofacRegistration();
        }

        private void AutofacRegistration()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterType<MusicStoreEntities>().As<IMusicStoreEntities>();
            builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly());

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        // Used to override how HttpContextWrapper gets resolved on requests to /@integrationtests.
        private class AutofacHttpContextModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.Register<HttpContextWrapper>(c => (HttpContextWrapper)HttpContext.Current.Items[typeof(HttpContextProxy)])
                    .As<HttpContextBase>()
                    .InstancePerHttpRequest();
            }
        }
    }
}