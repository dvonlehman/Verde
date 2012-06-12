using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Verde.Executor
{
    /// <summary>
    /// Base class for request executors. 
    /// </summary>
    /// <remarks>
    /// Starting with MvcExecutorScope but in theory a WebFormsRequestExectorContext could be implemented.
    /// </remarks>
    public abstract class ExecutorScope : IDisposable
    {
        private readonly HttpContextBase _httpContext;

        public ExecutorScope(ExecutorSettings settings)
        {
            this.Settings = settings;

            // ASP.Net appears to lazy load the ServerVariables. Ensure they are already loaded before executing 
            // the simulated request, otherwise an exception is thrown.
            var serverVariables = System.Web.HttpContext.Current.Request.ServerVariables;

            _httpContext = new ExecutorHttpContext(System.Web.HttpContext.Current, settings);

            // Execute the request
            this.Execute();
        }

        protected ExecutorSettings Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// The http context for the request being executed.
        /// </summary>
        public HttpContextBase HttpContext
        {
            get { return _httpContext; }
        }

        /// <summary>
        /// The response text of the executed request.
        /// </summary>
        public string ResponseText
        {
            get;
            protected set;
        }

        public void Dispose()
        {
            // Now that we are done with the request, set it back to the original state.
            _httpContext.Response.Clear();
            _httpContext.Response.StatusCode = 200;
        }

        protected abstract void Execute();
    }
}
