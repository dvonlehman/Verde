using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Verde.Executor
{
    /// <summary>
    /// Execute an MVC request against the site.
    /// </summary>
    public class MvcRequestExecutor : IDisposable
    {
        private readonly HttpContextBase _httpContext;

        public MvcRequestExecutor(Uri requestUrl) : this(new RequestExecutorSettings(requestUrl.ToString())) { }

        public MvcRequestExecutor(string path, string query) : this(new RequestExecutorSettings(path, query)) { }

        public MvcRequestExecutor(string path) : this(new RequestExecutorSettings(path)) { }

        public MvcRequestExecutor(RequestExecutorSettings settings)
        {
            // ASP.Net appears to lazy load the ServerVariables. Ensure they are already loaded before executing 
            // the simulated request, otherwise an exception is thrown.
            var serverVariables = System.Web.HttpContext.Current.Request.ServerVariables;

            _httpContext = new ExecutorHttpContext(System.Web.HttpContext.Current, settings);
        }

        public void Execute()
        {
            var routeData = RouteTable.Routes.GetRouteData(_httpContext);

            this.Action = (string)routeData.Values["action"];
            string controllerName = (string)routeData.Values["controller"];

            // Temporarily override the ControllerFactory to our own WrapperControllerFactory which allows 
            // us to subsequently use all of the Controller objects that get created within.
            var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
            var wrapperControllerFactory = new WrapperControllerFactory(controllerFactory);
            ControllerBuilder.Current.SetControllerFactory(wrapperControllerFactory);

            try
            {
                RequestContext requestContext = new RequestContext(_httpContext, routeData);
                var handler = new ExecutorMvcHandler(requestContext, controllerName);

                using (var writer = new StringWriter())
                {
                    System.Web.HttpContext.Current.Server.Execute(HttpHandlerUtil.WrapForServerExecute(handler), writer, false /* preserveForm */);
                    this.ResponseText = writer.ToString();
                }

                if (wrapperControllerFactory.CreatedControllers.Count > 0)
                {
                    this.Controller = wrapperControllerFactory.CreatedControllers[0];
                    this.ViewData = this.Controller.ViewData;
                }
            }
            finally
            {
                // Set the ControllerFactory back to the original value.
                ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            }
        }

        /// <summary>
        /// The response text of the simulated request.
        /// </summary>
        public string ResponseText
        {
            get;
            private set;
        }

        public string Action
        {
            get;
            private set;
        }

        /// <summary>
        /// The ViewData created by the simulated request.
        /// </summary>
        public ViewDataDictionary ViewData
        {
            get;
            private set;
        }

        /// <summary>
        /// The controller created by the simulated request.
        /// </summary>
        public Controller Controller
        {
            get;
            private set;
        }

        /// <summary>
        /// The HttpContext for the simulated request.
        /// </summary>
        public HttpContextBase HttpContext
        {
            get { return _httpContext; }
        }

        public void Dispose()
        {
            // Now that we are done with the request, set it back to the original state.
            _httpContext.Response.Clear();
            _httpContext.Response.StatusCode = 200;
        }
    }
}
