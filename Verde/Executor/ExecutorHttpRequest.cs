using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.UI;
using System.Web;

namespace Verde.Executor
{
    /// <summary>
    /// A simulated MVC http request to test.
    /// </summary>
    public class ExecutorHttpRequest : HttpRequestWrapper
    {
        private readonly ExecutorSettings _settings;
        private RequestContext _requestContext;
        private NameValueCollection _queryString;
        
        public ExecutorHttpRequest(HttpRequest originalRequest, ExecutorSettings settings) : base(originalRequest)
        {
            _settings = settings;
            if (_settings.Url == null)
                throw new ArgumentException("The SimulatedRequestSettings must specifiy a Url.");

            if (_settings.Cookies == null)
                _settings.Cookies = new HttpCookieCollection();

            _queryString = (settings.Url.Query.Length > 1) ? ParseQueryString(_settings.Url.Query.Substring(1)) : new NameValueCollection();
            if (_settings.Form == null)
                _settings.Form = new NameValueCollection();

            if (_settings.Files == null)
                _settings.Files = new ExecutorHttpFileCollection();

            if (_settings.RequestHeaders == null)
                _settings.RequestHeaders = new NameValueCollection();

            _requestContext = new RequestContext();

            if (string.IsNullOrEmpty(_settings.HttpMethod))
                _settings.HttpMethod = (_settings.Form.Count > 0) ? "POST" : "GET";
        }

        public override HttpCookieCollection Cookies
        {
            get { return _settings.Cookies ?? base.Cookies; }
        }

        public override bool IsAuthenticated
        {
            get
            {
                return _settings.IsAuthenticated;
            }
        }

        /// <summary>
        /// <see cref="HttpRequestBase.Url"/>
        /// </summary>
        public override Uri Url
        {
            get { return _settings.Url; }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return "~" + _settings.Url.LocalPath; }
        }

        public override string  CurrentExecutionFilePath
        {
            //TODO: Need to set this property correctly.
	        get { return "/"; }
	    }

        public override NameValueCollection Form
        {
            get { return _settings.Form; }
        }

        public override NameValueCollection QueryString
        {
            get { return _queryString; }
        }

        public override string HttpMethod
        {
            get { return _settings.HttpMethod; }
        }

        public override Uri UrlReferrer
        {
            get { return _settings.Referrer; }
        }

        public override string UserAgent
        {
            get { return _settings.UserAgent; }
        }

        public override string Path
        {
            get { return _settings.Url.LocalPath; }
        }

        public override string PathInfo
        {
            get { return string.Empty; }    
        }

        public override NameValueCollection Headers
        {
            get { return _settings.RequestHeaders; }
        }

        public override bool IsLocal
        {
            get { return true; }
        }

        public override HttpFileCollectionBase Files
        {
            get { return _settings.Files; }
        }

        public override string ContentType
        {
            get { return String.IsNullOrEmpty(_settings.RequestContentType) ? "text/html" : _settings.RequestContentType; }
            set { _settings.RequestContentType = value; }
        }

        public override RequestContext RequestContext
        {
            get { return _requestContext; }
        }

        private NameValueCollection ParseQueryString(string value)
        {
            if (String.IsNullOrEmpty(value)) return new NameValueCollection();

            string[] pairs = value.Split('&');
            var coll = new NameValueCollection(pairs.Length, StringComparer.CurrentCultureIgnoreCase);
            for (var i = 0; i < pairs.Length; i++)
            {
                string[] keyValue = pairs[i].Split('=');
                if (keyValue.Length == 2)
                    coll[keyValue[0]] = HttpUtility.UrlDecode(keyValue[1]);
            }

            return coll;
        }
    }
}
