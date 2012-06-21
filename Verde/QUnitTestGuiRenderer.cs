using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.IO;

namespace Verde
{
    /// <summary>
    /// Test Gui renderer that uses the QUnit JavaScript test framework.
    /// </summary>
    public class QUnitTestGuiRenderer : ITestGuiRenderer
    {
        void ITestGuiRenderer.Render(HttpContextBase context)
        {
            string html;
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Verde.Content.qunit-gui.htm"))
            using (var reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            // Get the absolute URL of the request to the GUI. Just to be safe ensure that we have a trailing 
            // slash to ensure the final URL to qunit-css and qunit-script resolves correctly.
            //var url = new UriBuilder(context.Request.Url);
            //url.Query = string.Empty;
            //string absoluteUrl = url.Uri.ToString();

            html = html.Replace("@@TITLE@@", Setup.CurrentSettings.GuiPageTitle);
            html = html.Replace("@@HEADER@@", Setup.CurrentSettings.GuiHeaderText);
            html = html.Replace("@@PATH@@", context.Request.Url.LocalPath.TrimEnd('/'));
            context.Response.Write(html);
        }
    }
}
