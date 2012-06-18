using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Verde.Executor
{
    /// <summary>
    /// Wrapper dependency resolver that allows us to control the HttpContextBase that is resolved.
    /// </summary>
    internal class WrapperDependencyResolver : IDependencyResolver
    {
        private readonly IDependencyResolver _original;
        private readonly HttpContextBase _httpContext;

        public WrapperDependencyResolver(IDependencyResolver original, HttpContextBase httpContext)
        {
            _original = original;
            _httpContext = httpContext;
        }

        object IDependencyResolver.GetService(Type serviceType)
        {
            if (serviceType == typeof(HttpContextBase))
                return _httpContext;
            else
                return _original.GetService(serviceType);
        }

        IEnumerable<object> IDependencyResolver.GetServices(Type serviceType)
        {
            return (IEnumerable<object>)_original.GetServices(serviceType);
        }
    }
}
