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
        void ITestGuiRenderer.Render(TextWriter writer)
        {
            string html;
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Verde.Content.qunit-gui.htm"))
            using (var reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            html = html.Replace("@@TITLE@@", Setup.CurrentSettings.GuiPageTitle);
            html = html.Replace("@@HEADER@@", Setup.CurrentSettings.GuiHeaderText);
            html = html.Replace("@@PATH@@", Setup.CurrentSettings.RoutePath);
            writer.Write(html);
        }
    }
}
