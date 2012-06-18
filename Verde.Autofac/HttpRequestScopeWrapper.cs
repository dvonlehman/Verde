using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Verde.Executor;

namespace Verde.Autofac
{
    internal class HttpRequestScopeWrapper : HttpRequestWrapper
    {
        public HttpRequestScopeWrapper(HttpRequest request) : base(request) { }

        public override NameValueCollection QueryString
        {
            get
            {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.QueryString : base.QueryString;
            }
        }

        public override HttpCookieCollection Cookies
        {
            get
            {
                return (ExecutorScope.Current != null) ? 
                    ExecutorScope.Current.HttpContext.Request.Cookies : base.Cookies;
            }
        }

        public override bool IsAuthenticated
        {
            get
            {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.IsAuthenticated : base.IsAuthenticated;
            }
        }

        public override Uri Url
        {
            get 
            {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.Url : base.Url;
            }
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.AppRelativeCurrentExecutionFilePath : base.AppRelativeCurrentExecutionFilePath;
            }
        }

        public override NameValueCollection Form
        {
            get {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.Form : base.Form;
            }
        }

        public override string HttpMethod
        {
            get 
            {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.HttpMethod : base.HttpMethod;
            }
        }

        public override Uri UrlReferrer
        {
            get {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.UrlReferrer : base.UrlReferrer;
                }
        }

        public override string UserAgent
        {
            get
            {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.UserAgent : base.UserAgent;
            }
        }

        public override string Path
        {
            get {
                return (ExecutorScope.Current != null) ?
                    ExecutorScope.Current.HttpContext.Request.Path : base.Path;
            }
        }

        //public override string PathInfo
        //{
        //    get { return string.Empty; }
        //}

        //public override NameValueCollection Headers
        //{
        //    get { return _settings.RequestHeaders; }
        //}

        //public override bool IsLocal
        //{
        //    get { return true; }
        //}

        //public override HttpFileCollectionBase Files
        //{
        //    get { return _settings.Files; }
        //}

        //public override string ContentType
        //{
        //    get { return String.IsNullOrEmpty(_settings.RequestContentType) ? "text/html" : _settings.RequestContentType; }
        //    set { _settings.RequestContentType = value; }
        //}

        //public override RequestContext RequestContext
        //{
        //    get { return _requestContext; }
        //}
    }
}
