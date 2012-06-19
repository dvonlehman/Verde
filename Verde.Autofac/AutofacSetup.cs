using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Verde;

namespace Verde.Autofac
{
    /// <summary>
    /// Setup Verde framework to be Autofac aware
    /// </summary>
    public static class AutofacSetup
    {
        /// <summary>
        /// Initialize the Verde <see cref="Settings"/> for Autofac.
        /// </summary>
        /// <param name="settings"></param>
        public static void Initialize(Verde.Settings settings)
        {
            settings.BeginExecuteTestsRequest += (sender, e) =>
            {
                var resolver = DependencyResolver.Current as AutofacDependencyResolver;
                if (resolver == null)
                    return;

                // Register the OverrideWebTypesModule so we can explicitly control how HttpContextBase gets resolved.
                new OverrideWebTypesModule().Configure(resolver.ApplicationContainer.ComponentRegistry);
            };
        }
    }
}
