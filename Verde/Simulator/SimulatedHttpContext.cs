using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Verde.Simulator
{
    /// <summary>
    /// A simulated HttpContext to use to test with.
    /// </summary>
    public class SimulatedHttpContext : HttpContextBase
    {
        private readonly RequestSimulatorSettings _settings;


        public SimulatedHttpContext(RequestSimulatorSettings settings)
        {
            _settings = settings;

            if (_settings.HttpRequest == null)
                _settings.HttpRequest = new SimulatedHttpRequest(settings);

            if (_settings.SessionState == null)
                _settings.SessionState = new SimulatedHttpSessionState(settings);

            if (_settings.HttpContextItems == null)
                _settings.HttpContextItems = new Hashtable();

            if (_settings.HttpResponse == null)
                _settings.HttpResponse = new SimulatedHttpResponse(settings);

            if (_settings.ApplicationState == null)
                _settings.ApplicationState = new HttpApplicationStateWrapper(HttpContext.Current.Application);

            _settings.HttpRequest.RequestContext.HttpContext = this;
            _settings.HttpRequest.RequestContext.RouteData = RouteTable.Routes.GetRouteData(this);
        }

        public override bool IsCustomErrorEnabled
        {
            get { return false; }
        }

        public override HttpRequestBase Request
        {
            get { return _settings.HttpRequest; }
        }

        public override HttpResponseBase Response
        {
            get { return _settings.HttpResponse; }
        }

        public override HttpSessionStateBase Session
        {
            get { return _settings.SessionState; }
        }

        public override IDictionary Items
        {
            get { return _settings.HttpContextItems; }
        }

        public override HttpApplicationStateBase Application
        {
            get { return _settings.ApplicationState; }
        }
    }  
}
