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
    public class MvcExecutorScope : ExecutorScope
    {
        public MvcExecutorScope(string path, string query) : base(new ExecutorSettings(path, query)) { }

        public MvcExecutorScope(string path) : base(new ExecutorSettings(path)) { }

        public MvcExecutorScope(ExecutorSettings settings) : base(settings) { }
        

        protected override void Execute()
        {
            var routeData = base.HttpContext.Request.RequestContext.RouteData;

            this.Action = (string)routeData.Values["action"];
            string controllerName = (string)routeData.Values["controller"];

            // Temporarily override the ControllerFactory to our own WrapperControllerFactory which allows 
            // us to subsequently use all of the Controller objects that get created within.
            var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
            var wrapperControllerFactory = new WrapperControllerFactory(controllerFactory);
            ControllerBuilder.Current.SetControllerFactory(wrapperControllerFactory);

            try
            {
                var handler = new ExecutorMvcHandler(base.HttpContext.Request.RequestContext, controllerName);

                using (var writer = new StringWriter())
                {
                    var wrappedHandler = HttpHandlerUtil.WrapForServerExecute(handler);
                    try
                    {
                        System.Web.HttpContext.Current.Server.Execute(wrappedHandler, writer, false /* preserveForm */);
                    }
                    catch (Exception e)
                    {
                        throw new ExecutorScopeException(FindInnermostException(e));
                    }

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

        private Exception FindInnermostException(Exception e)
        {
            Exception current = e;
            while (current.InnerException != null)
                current = current.InnerException;

            return current;
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

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
