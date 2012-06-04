using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Verde.Executor
{
    internal class ExecutorMvcHandler : MvcHandler
    {
        private readonly string _controllerName;

        public ExecutorMvcHandler(RequestContext requestContext, string controllerName) : base(requestContext)
        {
            _controllerName = controllerName;
        }

        protected override void ProcessRequest(HttpContextBase httpContext)
        {
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            this.Controller = factory.CreateController(base.RequestContext, _controllerName);

            try
            {
                this.Controller.Execute(base.RequestContext);
            }
            finally
            {
                factory.ReleaseController(this.Controller);
            }
        }

        protected override IAsyncResult BeginProcessRequest(HttpContextBase httpContext, AsyncCallback callback, object state)
        {
            return base.BeginProcessRequest(httpContext, callback, state);
        }

        protected override void AddVersionHeader(HttpContextBase httpContext)
        {
        }

        public IController Controller
        {
            get;
            private set;
        }
    }
}
