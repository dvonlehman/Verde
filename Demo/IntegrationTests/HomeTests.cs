using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using Verde.Simulator;

namespace Verde.Demo.IntegrationTests
{
    [TestFixture]
    public class HomeTests
    {
        [Test]
        public void DummyTest()
        {
            Assert.IsTrue(true, "Yep it's true");

            var url = new UriBuilder(HttpContext.Current.Request.Url);
            url.Path = "Home";
            url.Query = string.Empty;

            var simulator = new MvcRequestSimulator(url.Uri).Execute();
            Assert.IsTrue(simulator.ResponseText.IndexOf("ASP.NET MVC") != -1);
        }
    }
}