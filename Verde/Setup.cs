using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verde
{
    /// <summary>
    /// Used to initialize the IntegrationTester.
    /// </summary>
    public static class Setup
    {
        private static Settings _settings;
        private static bool _initialized;

        /// <summary>
        /// Initialize the integration tester.
        /// </summary>
        /// <remarks>
        /// This should be invoked from the Application_Start method in Global.asax.
        /// </remarks>
        public static void Initialize(Settings settings)
        {
            if (_initialized)
                throw new InvalidOperationException("Initialize method should only be called once.");

            _settings = settings;
            _settings.Validate();

            IntegrationTestHandler.RegisterRoutes();

            _initialized = true;
        }

        internal static Settings CurrentSettings
        {
            get { return _settings; }
        }
    }
}
