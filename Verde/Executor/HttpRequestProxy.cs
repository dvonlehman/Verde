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
    internal class HttpRequestProxy : HttpRequestWrapper
    {
        private Uri _url;
        private HttpCookieCollection _cookies;
        private NameValueCollection _form;
        private NameValueCollection _headers;
        private string _httpMethod;
        private bool _isAuthenticated;
        private RequestContext _requestContext;
        private NameValueCollection _queryString;
        private Uri _referrer;
        private string _userAgent;
        //private readonly HttpFileCollectionProxy _files;
        private string _contentType;

        public HttpRequestProxy(HttpRequest originalRequest)
            : base(originalRequest)
        {
           // _files = new HttpFileCollectionProxy(originalRequest.Files);
            _requestContext = new RequestContext();
        }

        internal void OverrideRequestState(ExecutorSettings settings)
        {
            if (settings.Url == null)
                throw new ArgumentException("The SimulatedRequestSettings must specifiy a Url.");

            _url = settings.Url;
            _cookies = (settings.Cookies != null) ? settings.Cookies : new HttpCookieCollection();
            _queryString = (settings.Url.Query.Length > 1) ? ParseQueryString(settings.Url.Query.Substring(1)) : new NameValueCollection();
            _form = (settings.Form  != null) ? settings.Form : new NameValueCollection();
            _headers = (settings.RequestHeaders != null) ? settings.RequestHeaders : new NameValueCollection();
            _httpMethod = !String.IsNullOrEmpty(settings.HttpMethod) ? settings.HttpMethod : (_form.Count > 0 ? "POST" : "GET");
            _isAuthenticated = settings.IsAuthenticated;    
            _userAgent = settings.UserAgent;
            _contentType = !String.IsNullOrEmpty(settings.RequestContentType) ? settings.RequestContentType : "text/html";
                    
            //if (_settings.Files == null)
            //    _settings.Files = new ExecutorHttpFileCollection();
        }

        public override HttpCookieCollection Cookies
        {
            get { return ExecutorScope.Current != null ? _cookies : base.Cookies; }
        }

        public override bool IsAuthenticated
        {
            get
            {
                return ExecutorScope.Current != null ? _isAuthenticated : base.IsAuthenticated;
            }
        }

        /// <summary>
        /// <see cref="HttpRequestBase.Url"/>
        /// </summary>
        public override Uri Url
        {
            get { return ExecutorScope.Current != null ? _url : base.Url; }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return ExecutorScope.Current != null ? ("~" + _url.LocalPath) : base.AppRelativeCurrentExecutionFilePath; }
        }

        public override string CurrentExecutionFilePath
        {
            //TODO: Need to set this property correctly.
            get { return "/"; }
        }

        public override NameValueCollection Form
        {
            get { return ExecutorScope.Current != null ? _form : base.Form; }
        }

        public override NameValueCollection QueryString
        {
            get { return ExecutorScope.Current != null ? _queryString : base.QueryString; }
        }

        public override string HttpMethod
        {
            get { return ExecutorScope.Current != null ? _httpMethod : base.HttpMethod; }
        }

        public override Uri UrlReferrer
        {
            get { return ExecutorScope.Current != null ? _referrer : base.UrlReferrer; }
        }

        public override string UserAgent
        {
            get { return (ExecutorScope.Current != null && !string.IsNullOrEmpty(_userAgent)) ? _userAgent : base.UserAgent; }
        }

        public override string Path
        {
            get { return ExecutorScope.Current != null ? _url.LocalPath : base.Path; }
        }

        public override string PathInfo
        {
            get { return string.Empty; }
        }

        public override NameValueCollection Headers
        {
            get { return ExecutorScope.Current != null ? _headers : base.Headers; }
        }

        public override bool IsLocal
        {
            get { return true; }
        }

        //public override HttpFileCollectionBase Files
        //{
        //    get { return _files; }        
        //}
        
        public override string ContentType
        {
            get { return ExecutorScope.Current != null ? _contentType : base.ContentType; }
            set { _contentType = value; }
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
