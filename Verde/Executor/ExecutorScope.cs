using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Verde;

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
        private readonly HttpContext _realHttpContext;
        private readonly HttpContextBase _httpContext;

        //public const string InChildExecutorScopeKey = "InChildExecutorScope";
        //public const string VerdeHttpContextKey = "VerdeHttpContext";
        
        public ExecutorScope(ExecutorSettings settings)
        {
            //Important that this line comes first so that ExecutorScope.Current does not return null.
            Current = this;

            HttpContextProxy.Current.OverrideContextState(settings);

            //_realHttpContext = System.Web.HttpContext.Current;

            //// Write a value to the HttpContext.Items collection as a marker to indicate 
            //// that we are in a child executor scope.
            //_realHttpContext.Items[InChildExecutorScopeKey] = true;

            // ASP.Net appears to lazy load the ServerVariables. Ensure they are already loaded before executing 
            // the simulated request, otherwise an exception is thrown.
            //var serverVariables = System.Web.HttpContext.Current.Request.ServerVariables;

            //_httpContext = new ExecutorHttpContext(settings);
            //_realHttpContext.Items[VerdeHttpContextKey] = _httpContext;

            foreach (var handler in Setup.CurrentSettings.ExecutorScopeCreatedHandlers)
                handler(this, EventArgs.Empty);

            // Execute the request
            this.Execute();
        }

        /// <summary>
        /// The http context for the request being executed.
        /// </summary>
        public HttpContextBase HttpContext
        {
            get { return HttpContextProxy.Current; }
        }

        /// <summary>
        /// The response text of the executed request.
        /// </summary>
        public string ResponseText
        {
            get;
            protected set;
        }

        public virtual void Dispose()
        {
            // Now that we are done with the request, set it back to the original state.
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.StatusCode = 200;
            //_realHttpContext.Items.Remove(InChildExecutorScopeKey);
            //_realHttpContext.Items.Remove(VerdeHttpContextKey);

            foreach (var handler in Setup.CurrentSettings.ExecutorScopeDisposedHandlers)
                handler(this, EventArgs.Empty);

            Current = null;
        }

        protected abstract void Execute();

        /// <summary>
        /// Get the current ExecutorScope.
        /// </summary>
        public static ExecutorScope Current
        {
            get { return System.Web.HttpContext.Current.Items[typeof(ExecutorScope)] as ExecutorScope; }
            private set { System.Web.HttpContext.Current.Items[typeof(ExecutorScope)] = value; }
        }
    }
}
