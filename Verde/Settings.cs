using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;

namespace Verde
{
    /// <summary>
    /// Used to configure the MvcIntegrationTester.
    /// </summary>
    public class Settings
    {
        private List<EventHandler> _scopeCreatedEventHandlers = new List<EventHandler>();
        private List<EventHandler> _scopeDisposedEventHandlers = new List<EventHandler>();
        private List<EventHandler> _beginExecuteTestRequestHandlers = new List<EventHandler>();

        /// <summary>
        /// Set the assembly that contains the integration tests.
        /// </summary>
        /// <param name="assembly"></param>
        /// <remarks>Ideally we could support multiple assemblies, but starting with just one for now.</remarks>
        [JsonIgnore]
        public Assembly TestsAssembly { get; set; }

        /// <summary>
        /// Set the path to use in the URL to the integration test runner.
        /// </summary>
        public string RoutePath { get; set; }

        /// <summary>
        /// Set the GUI renderer.
        /// </summary>
        /// <remarks>
        /// By default the <see cref="QunitTestGuidRenderer"/> will be used.
        /// </remarks>
        [JsonIgnore]
        public ITestGuiRenderer GuiRenderer { get; set; }

        /// <summary>
        /// The test runner.
        /// </summary>
        /// <remarks>
        /// By default uses the NUnitTestRunner.
        /// </remarks>
        [JsonIgnore]
        public ITestRunner TestRunner { get; set; }

        /// <summary>
        /// The value of the &lt;title&gt; element on the integration test GUI page.
        /// </summary>
        public string GuiPageTitle { get; set; }

        /// <summary>
        /// The value of the banner on the test GUI page.
        /// </summary>
        public string GuiHeaderText { get; set; }

        /// <summary>
        /// The number of seconds to allow for the request to execute tests should be allowed to take.
        /// </summary>
        /// <remarks>The default is 90 seconds.</remarks>
        public int ExecuteTimeout { get; set; }

        /// <summary>
        /// Function to invoke when a request to the integration test handler is initiated. The delegate
        /// should return true if access is allowed, otherwise false.
        /// </summary>
        /// <remarks>
        /// This is useful in case the integration test routes are available in a production 
        /// environment but you need to protect just anyone from accessing the URL. The function could
        /// check for a special authorization cookie, check the roles of the current principal, etc.
        /// </remarks>
        [JsonIgnore]
        public Func<HttpContext, bool> AuthorizationCheck { get; set; }

        internal IList<EventHandler> ExecutorScopeCreatedHandlers
        {
             get { return _scopeCreatedEventHandlers; }
        }

        internal IList<EventHandler> ExecutorScopeDisposedHandlers
        {
            get { return _scopeDisposedEventHandlers; }
        }

        internal IList<EventHandler> BeginExecuteTestsRequestHandlers
        {
            get { return _beginExecuteTestRequestHandlers; }
        }

        /// <summary>
        /// Event that fires when a new <see cref="Verde.Executor.ExecutorScope"/> is created.
        /// </summary>
        public event EventHandler ExecutorScopeCreated
        {
            add { _scopeCreatedEventHandlers.Add(value); }
            remove { _scopeCreatedEventHandlers.Remove(value); }
        }

        /// <summary>
        /// Event that fires when an <see cref="Verde.Executor.ExecutorScope"/> is disposed.
        /// </summary>
        public event EventHandler ExecutorScopeDisposed
        {
            add { _scopeDisposedEventHandlers.Add(value); }
            remove { _scopeDisposedEventHandlers.Remove(value); }
        }

        /// <summary>
        /// Event that fires when the Http request to execute one or more tests begins.
        /// </summary>
        public event EventHandler BeginExecuteTestsRequest
        {
            add { _beginExecuteTestRequestHandlers.Add(value); }
            remove { _beginExecuteTestRequestHandlers.Remove(value); }
        }

        internal void Validate()
        {
            if (this.TestsAssembly == null)
                throw new InvalidOperationException("Missing TestAssemby, be sure Settings.TestAssembly has been set, generally in Application_Start of Global.asax.");

            // Set default values for any missing properties.
            if (String.IsNullOrEmpty(RoutePath))
                RoutePath = "@integrationtests";
            if (String.IsNullOrEmpty(GuiPageTitle))
                GuiPageTitle = "Verde Integration Tester";
            if (String.IsNullOrEmpty(GuiHeaderText))
                GuiHeaderText= "Verde Integration Tester";
            if (GuiRenderer == null)
                GuiRenderer = new QUnitTestGuiRenderer();
            if (TestRunner == null)
                TestRunner = new NUnitTestRunner(this);
            if (ExecuteTimeout <= 0)
                ExecuteTimeout = 90;
        }
    }
}
