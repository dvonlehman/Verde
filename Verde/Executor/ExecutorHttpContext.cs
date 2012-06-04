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
        
        public ExecutorHttpContext(HttpContext httpContext, RequestExecutorSettings settings): base(httpContext)
        {
            _settings = settings;

            if (_settings.HttpRequest == null)
                _settings.HttpRequest = new ExecutorHttpRequest(httpContext.Request, settings);

            if (_settings.SessionState == null)
                _settings.SessionState = new ExecutorHttpSessionState(settings);

            if (_settings.HttpContextItems == null)
                _settings.HttpContextItems = new Hashtable();

            //if (_settings.HttpResponse == null)
            //    _settings.HttpResponse = new SimulatedHttpResponse(httpContext.Response, settings);

            if (_settings.User == null)
                this.User = DefaultIdentity;

            _settings.HttpRequest.RequestContext.HttpContext = this;
            _settings.HttpRequest.RequestContext.RouteData = RouteTable.Routes.GetRouteData(this);
        }
      
        public override HttpRequestBase Request
        {
            get { return _settings.HttpRequest ?? base.Request; }
        }

        //public override HttpResponseBase Response
        //{
        //    get { return _settings.HttpResponse ?? base.Response; }
        //}

        public override HttpSessionStateBase Session
        {
            get { return _settings.SessionState ?? base.Session; }
        }
    }     
}