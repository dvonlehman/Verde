using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Verde;
using Verde.Autofac;
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

            // Since we are using Autofac for DI, we need to make the Verde framework aware.
            AutofacSetup.Initialize(settings);

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

            builder.RegisterType<MusicStoreEntities>().As<IMusicStoreEntities>().SingleInstance();
            builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly());

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}