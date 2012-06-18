using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Routing;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Verde
{
    /// <summary>
    /// Route and HttpHandler for @integrationtest/* requests.
    /// </summary>
    /// <remarks>
    /// Implementation of this class borrows heavily from the MinProfilerHandler from the excellent MvcMiniProfiler project.
    /// http://miniprofiler.com/
    /// </remarks>
    internal class IntegrationTestHandler : IHttpHandler, IRouteHandler
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { 
            Formatting= Newtonsoft.Json.Formatting.Indented, 
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            var contextBase = new HttpContextWrapper(context);

            if (Setup.CurrentSettings.AuthorizationCheck != null)
            {
                if (!Setup.CurrentSettings.AuthorizationCheck(context))
                    throw new HttpException(403, "Access denied");
            }

            string path = context.Request.AppRelativeCurrentExecutionFilePath;

            switch (Path.GetFileNameWithoutExtension(path).ToLowerInvariant().TrimStart('/').TrimEnd('/'))
            {
                case "qunit-css":
                    contextBase.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                    contextBase.Response.ContentType = "text/css";
                    contextBase.Response.Write(LoadStaticContentResource("qunit-css.css"));

                    break;
                case "qunit-script":
                    contextBase.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                    contextBase.Response.ContentType = "text/javascript";
                    contextBase.Response.Write(LoadStaticContentResource("qunit-script.js"));

                    break;
                case "tests":
                    contextBase.Response.ContentType = "application/json";
                    contextBase.Response.Write(JsonConvert.SerializeObject(new
                    {
                        settings= Setup.CurrentSettings, 
                        fixtures= Setup.CurrentSettings.TestRunner.LoadTestFixtures()
                    }, _serializerSettings));

                    //RenderTestsJson(context.Response.Output);
                    break;
                case "execute":
                    foreach (var handler in Setup.CurrentSettings.BeginExecuteTestsRequestHandlers)
                        handler(this, EventArgs.Empty);

                    ExecuteTests(contextBase);
                    break;
                case "":
                default:
                    // Render the GUI test console.
                    Setup.CurrentSettings.GuiRenderer.Render(contextBase);
                    break;
            }
        }

        private void ExecuteTests(HttpContextBase context)
        {
            var results = Setup.CurrentSettings.TestRunner.Execute(
                context.Request.QueryString["fixture"],
                context.Request.QueryString["test"]);
            
            if (results.Tests.Count == 0)
                throw new InvalidOperationException("No tests found to execute.");

            context.Response.ContentType = "application/json";
            if (results.Failed)
                context.Response.StatusCode = 500;

            context.Response.Write(JsonConvert.SerializeObject(results, _serializerSettings));
        }

        private string LoadStaticContentResource(string resourceName)
        {
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Verde.Content." + resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        /// <summary>
        /// Dynamically register the MVC routes for our integration test URLs.
        /// </summary>
        internal static void RegisterRoutes()
        {
            var routes = RouteTable.Routes;
            var handler = new IntegrationTestHandler();

            var routePaths = new string[] { 
                Setup.CurrentSettings.RoutePath,                    // the main test GUI
                Setup.CurrentSettings.RoutePath + "/qunit-css",     // the GUI stylesheet for the qunit renderer
                Setup.CurrentSettings.RoutePath + "/qunit-script",  // the GUI javascript for the qunit renderer
                Setup.CurrentSettings.RoutePath + "/tests",         // the javascript containing all our tests
                Setup.CurrentSettings.RoutePath + "/execute"        // execute a single test 
            };

            using (routes.GetWriteLock())
            {
                foreach (var path in routePaths) 
                {
                    var route = new Route(path, handler)
                    {
                        // we have to specify these, so no MVC route helpers will match, e.g. @Html.ActionLink("Home", "Index", "Home")
                        Defaults = new RouteValueDictionary(new { controller = "IntegrationTestHandler", action = "ProcessRequest" }),
                        Constraints = new RouteValueDictionary(new { controller = "IntegrationTestHandler", action = "ProcessRequest" })
                    };

                    routes.Insert(0, route);
                }
            }
        }
    }
}
