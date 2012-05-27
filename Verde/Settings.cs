using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using NUnit.Core;
using Newtonsoft.Json;

namespace Verde
{
    /// <summary>
    /// Used to configure the MvcIntegrationTester.
    /// </summary>
    public class Settings
    {
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
        /// The number of seconds before an individual test should timeout.
        /// </summary>
        public int TestTimeout { get; set; }

        internal void Validate()
        {
            if (this.TestsAssembly == null)
                throw new InvalidOperationException("Missing TestAssemby, be sure Settings.TestAssembly has been set, generally in Application_Start of Global.asax.");

            // Set default values for any missing properties.
            if (String.IsNullOrEmpty(RoutePath))
                RoutePath = "@integrationtests";
            if (String.IsNullOrEmpty(GuiPageTitle))
                GuiPageTitle = "MvcIntegrationTester";
            if (String.IsNullOrEmpty(GuiHeaderText))
                GuiHeaderText= "Mvc Integration Tests";
            if (GuiRenderer == null)
                GuiRenderer = new QUnitTestGuiRenderer();
            if (TestRunner == null)
                TestRunner = new NUnitTestRunner(this);
            if (TestTimeout <= 0)
                TestTimeout = 5;
        }
    }
}
