using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Verde.Executor;
using Castle.DynamicProxy;

namespace Verde.Autofac
{
    internal class HttpContextLifetimeProxy : HttpContextWrapper
    {
        private static readonly HttpObjectInterceptor<ExecutorHttpRequest> _httpRequestInterceptor = 
            new HttpObjectInterceptor<ExecutorHttpRequest>(
                ()=> (ExecutorHttpRequest)ExecutorScope.Current.HttpContext.Request);

        private static readonly HttpObjectInterceptor<ExecutorHttpSessionState> _httpSessionInterceptor = 
            new HttpObjectInterceptor<ExecutorHttpSessionState>(
                ()=> (ExecutorHttpSessionState)ExecutorScope.Current.HttpContext.Session);
        
        private HttpRequestBase _request;
        private HttpSessionStateBase _session;

        public HttpContextLifetimeProxy(HttpContext context)
            : base(context)
        {

            var generator = new ProxyGenerator();

            _request = generator.CreateClassProxy<HttpRequestBase>(_httpRequestInterceptor);
            _session = generator.CreateClassProxy<HttpSessionStateBase>(_httpSessionInterceptor);
        }

        public override HttpRequestBase Request
        {
            get { return _request; }
        }

        public override HttpSessionStateBase Session
        {
	        get { return _session; }
        }

        private class HttpObjectInterceptor<T> : IInterceptor
        {
            private readonly IList<PropertyInfo> _interceptProperties;
            private readonly Func<T> _getRealObject;

            public HttpObjectInterceptor(Func<T> getRealObject)
            {
                _getRealObject = getRealObject;
                _interceptProperties = FindInterceptableProperties(typeof(T));
            }

            public void Intercept(IInvocation invocation)
            {
                var getter = _interceptProperties.FirstOrDefault(p => "get_" + p.Name == invocation.Method.Name);
                if (getter != null)
                {
                    // If we are in an ExecutorScope then return the value of the property on the ExecutorHttpRequest
                    // rather than the actual HttpContext.Current.
                    if (ExecutorScope.Current != null)
                        invocation.ReturnValue = getter.GetValue(_getRealObject(), null);
                }
            }
        }

        private static IList<PropertyInfo> FindInterceptableProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p=> {
                var getter = p.GetGetMethod();
                if (getter == null)
                    return false;
                return type.IsSubclassOf(getter.GetBaseDefinition().DeclaringType);
            }).ToList();
        }
    }
}
