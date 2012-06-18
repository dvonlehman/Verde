using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Builder;
using Verde.Executor;

namespace Verde.Autofac
{

    public class OverrideWebTypesModule : Module
    {
        /// <summary>
        /// Registers web abstractions with dependency injection.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="Autofac.ContainerBuilder"/> in which registration
        /// should take place.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method registers mappings between common current context-related
        /// web constructs and their associated abstract counterparts. See
        /// <see cref="Autofac.Integration.Mvc.AutofacWebTypesModule"/> for the complete
        /// list of mappings that get registered.
        /// </para>
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<HttpContextWrapper>(c=> new HttpContextLifetimeProxy(HttpContext.Current))
                .As<HttpContextBase>()
                .InstancePerHttpRequest();
                        
            //builder.Register(c => {
            //    if (ExecutorScope.Current != null)
            //        return ExecutorScope.Current.HttpContext;
            //    else
            //        return new HttpContextWrapper(HttpContext.Current);
            //}).As<HttpContextBase>()
            //    .InstancePerOwned<HttpContextBase>();

            //builder.Register(c => c.Resolve<HttpContextBase>().Request)
            //  .As<HttpRequestBase>()
            //  .InstancePerOwned<HttpRequestBase>();
        }
    }
}
