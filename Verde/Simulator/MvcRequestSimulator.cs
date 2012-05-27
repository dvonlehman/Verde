using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Verde.Simulator
{
    /// <summary>
    /// Used to simulate an HTTP request to the MVC application being tested.
    /// </summary>
    public class MvcRequestSimulator
    {
        private readonly HttpContextBase _httpContext;

        public MvcRequestSimulator(Uri requestUrl) : this(new RequestSimulatorSettings(requestUrl)) { }

        public MvcRequestSimulator(RequestSimulatorSettings settings)
        {
            _httpContext = new SimulatedHttpContext(settings);
        }

        public MvcRequestSimulator(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        /// <summary>
        /// Execute the request.
        /// </summary>
        public MvcRequestSimulator Execute()
        {
            var routeData = RouteTable.Routes.GetRouteData(_httpContext);

            this.Action = (string)routeData.Values["action"];
            string controllerName = (string)routeData.Values["controller"];

            var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
            this.Controller = (Controller)controllerFactory.CreateController(_httpContext.Request.RequestContext, controllerName);

            var controllerContext = new ControllerContext
            {
                Controller = this.Controller,
                HttpContext = _httpContext,
                RouteData = routeData
            };

            this.Controller.ControllerContext = controllerContext;

            // Need to explicitly set the ValueProvider ourselves. By default there is low level code in the MVC framework 
            // which will use HttpContext.Current which does not have the Form and QueryString values we want to test.
            // We want to make sure the values we've specified in the MockHttpContext are used.
            this.Controller.ValueProvider = UnvalidatedValueProviderFactory.GetValueProvider(controllerContext);

            ((IController)this.Controller).Execute(_httpContext.Request.RequestContext);

            _httpContext.Response.Output.Flush();
            var outputStream = _httpContext.Response.OutputStream;
            outputStream.Position = 0;
            using (var streamReader = new  StreamReader(outputStream))
            {
                this.ResponseText = streamReader.ReadToEnd();
            }

            return this;
        }
        
        public string Action
        {
            get;
            private set;
        }

        public Controller Controller
        {
            get;
            private set;
        }

        public string ResponseText
        {
            get;
            private set;
        }
    }
}
