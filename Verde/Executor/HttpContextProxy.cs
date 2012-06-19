using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using Castle.DynamicProxy;
using System.Security.Principal;

namespace Verde.Executor
{
    public class HttpContextProxy : HttpContextWrapper
    {
        private static readonly IPrincipal DefaultIdentity =
            new GenericPrincipal(new GenericIdentity("Anonymous"), new string[] { });

        private readonly HttpRequestProxy _request;
        private readonly Lazy<HttpSessionStateProxy> _session;
        private IDictionary _items;
        private IPrincipal _user;

        internal static void Initialize()
        {
            // ASP.Net appears to lazy load the ServerVariables. Ensure they are already loaded before executing 
            // the simulated request, otherwise an exception is thrown.
            var serverVariables = HttpContext.Current.Request.ServerVariables;

            HttpContext.Current.Items[typeof(HttpContextProxy)] = new HttpContextProxy(HttpContext.Current);
        }

        internal static HttpContextProxy Current
        {
            get { return HttpContext.Current.Items[typeof(HttpContextProxy)] as HttpContextProxy; }
        }

        private HttpContextProxy(HttpContext context) : base(context)
        {
            _request = new HttpRequestProxy(context.Request);
            _session = new Lazy<HttpSessionStateProxy>(() => new HttpSessionStateProxy(HttpContext.Current.Session));

            _user = DefaultIdentity;
            _items = new Hashtable();

            context.Items[typeof(HttpContextProxy)] = this;
        }

        public void OverrideContextState(ExecutorSettings settings)
        {
            this.User = (settings.User != null) ? settings.User : DefaultIdentity;
            _items = (settings.HttpContextItems != null) ? settings.HttpContextItems : new Hashtable();
            
            _request.OverrideRequestState(settings);
            _session.Value.OverrideSessionState(settings);

            _request.RequestContext.HttpContext = this;

            // Important that this line comes after we call OverrideReqeustState on the HttpRequestProxy
            _request.RequestContext.RouteData = RouteTable.Routes.GetRouteData(this);
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }

        public override HttpSessionStateBase Session
        {
            get { return _session.Value; }
        }

        public override IDictionary Items
        {
            get { return ExecutorScope.Current != null ? _items : base.Items; }
        }

        public override IPrincipal User
        {
            get { return ExecutorScope.Current != null ? _user : base.User; }
            set { _user = value; }
        }
    }
}
