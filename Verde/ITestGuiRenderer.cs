using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Verde
{
    /// <summary>
    /// Interface representing a class that renders a test GUI.
    /// </summary>
    public interface ITestGuiRenderer
    {
        /// <summary>
        /// Render the markup for the Gui to the writer.
        /// </summary>
        /// <param name="writer"></param>
        void Render(TextWriter writer);
    }
}