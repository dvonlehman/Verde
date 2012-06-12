using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Verde
{
    /// <summary>
    /// Attribute used to indicate a class that contains integration tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class IntegrationFixtureAttribute : Attribute
    {
        public int Sequence { get; set; }
    }

    /// <summary>
    /// Attribute used to mark an integration test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class IntegrationTestAttribute : Attribute
    {
    }
}
