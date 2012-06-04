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
using NUnit.Core;
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
            ContractResolver = new CamelCasePropertyNamesContractResolver()};

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            string path = context.Request.AppRelativeCurrentExecutionFilePath;

            switch (Path.GetFileNameWithoutExtension(path).ToLowerInvariant())
            {
                case "qunit-css":
                    context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                    context.Response.ContentType = "text/css";
                    context.Response.Write(LoadStaticContentResource("qunit-css.css"));

                    break;
                case "qunit-script":
                    context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                    context.Response.ContentType = "text/javascript";
                    context.Response.Write(LoadStaticContentResource("qunit-script.js"));

                    break;
                case "tests":
                    context.Response.ContentType = "application/json";
                    RenderTestsJson(context.Response.Output);
                    break;
                case "execute":
                    string testName = context.Request.QueryString["test"];
                    var results = Setup.CurrentSettings.TestRunner.Execute(testName);

                    if (results.Tests.Count == 0)
                        throw new InvalidOperationException("Test '" + testName + "' not found.");

                    context.Response.ContentType = "application/json";
                    if (results.Failed)
                        context.Response.StatusCode = 500;
                   
                    context.Response.Write(JsonConvert.SerializeObject(results, Formatting.None, _serializerSettings));
                    break;
                case "executeall":
                    break;
                case "":
                default:
                    // Render the GUI test console.
                    Setup.CurrentSettings.GuiRenderer.Render(context.Response.Output);
                    break;
            }
        }

        private void RenderTestsJson(TextWriter textWriter)
        {
            var json = new JsonTextWriter(textWriter);
            json.WriteStartObject();

            json.WritePropertyName("settings");
            json.WriteRawValue(JsonConvert.SerializeObject(Setup.CurrentSettings, Formatting.None,_serializerSettings));
           
            json.WritePropertyName("tests");
            json.WriteStartObject();

            foreach (var fixture in Setup.CurrentSettings.TestRunner.LoadTestFixtures())
            {
                json.WritePropertyName(fixture.Name);
                json.WriteStartArray();
                foreach (var testName in fixture.Tests)
                    json.WriteValue(testName);
                json.WriteEndArray();
            }

            json.WriteEndObject();
            json.WriteEndObject();
            json.Flush();
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
                Setup.CurrentSettings.RoutePath + "/execute",       // execute a single test 
                Setup.CurrentSettings.RoutePath + "/executeall"     // execute the entire test suite
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
