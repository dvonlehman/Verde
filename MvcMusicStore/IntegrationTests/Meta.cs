using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Verde;
using Verde.Executor;

namespace MvcMusicStore.IntegrationTests
{
    [IntegrationFixture]
    public class Meta
    {
        [IntegrationTest]
        public void Autofac_ResolveHttpRequest_UsesCorrectLifetimeScope()
        {
            using (var scope = new MvcExecutorScope("Meta", "testparam=1"))
            {
                //var context = DependencyResolver.Current.GetService<HttpContextBase>();

                var request = DependencyResolver.Current.GetService<HttpRequestBase>();
                Assert.AreEqual("1", request.QueryString["testparam"]);
            }

            using (var scope = new MvcExecutorScope("Meta", "testparam=2"))
            {
                //var context = DependencyResolver.Current.GetService<HttpContextBase>();

                var request = DependencyResolver.Current.GetService<HttpRequestBase>();
                Assert.AreEqual("2", request.QueryString["testparam"]);
            }

            // Now that we are outside the executor scope, resolving the HttpRequestBase should return to the 
            // actual HttpRequest.
            //Assert.IsNullOrEmpty(DependencyResolver.Current.GetService<HttpRequestBase>().QueryString["scope"]);
        }
    }
}