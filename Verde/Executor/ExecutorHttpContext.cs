using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Security.Principal;

namespace Verde.Executor
{
    /// <summary>
    /// A simulated HttpContext to use to test with.
    /// </summary>
    public class ExecutorHttpContext : HttpContextWrapper
    {
        private static readonly IPrincipal DefaultIdentity =
            new GenericPrincipal(new GenericIdentity("Anonymous"), new string[] { });

        private readonly RequestExecutorSettings _settings;
        private readonly IHttpHandler _currentHandler;
        private readonly HttpRequestBase _request;
        private readonly HttpSessionStateBase _session;
        
        public ExecutorHttpContext(HttpContext httpContext, RequestExecutorSettings settings): base(httpContext)
        {
            _settings = settings;

            _request = new ExecutorHttpRequest(httpContext.Request, settings);
            _session = new ExecutorHttpSessionState(settings);

            if (_settings.HttpContextItems == null)
                _settings.HttpContextItems = new Hashtable();

            this.User = _settings.User ?? DefaultIdentity;

            _request.RequestContext.HttpContext = this;
            _request.RequestContext.RouteData = RouteTable.Routes.GetRouteData(this);
        }
      
        public override HttpRequestBase Request
        {
            get { return _request; }
        }

        public override HttpSessionStateBase Session
        {
            get { return _session; }
        }

        public override IDictionary Items
        {
            get
            {
                return _settings.HttpContextItems;
            }
        }
    }     
}