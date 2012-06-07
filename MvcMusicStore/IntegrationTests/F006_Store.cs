using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Models;
using NUnit.Framework;
using Verde.Executor;

namespace MvcMusicStore.IntegrationTests
{
    [TestFixture]
    public class F006_Store
    {
        [Test]
        public void ControllerEvents_DoExecute()
        {
            using (var executor = new MvcRequestExecutorContext("Store"))
            {
                var model = executor.ViewData.Model as IList<Genre>;
                Assert.IsNotNull(model);

                Assert.IsTrue((bool)executor.HttpContext.Items["OnActionExecuting"]);
                Assert.IsTrue((bool)executor.HttpContext.Items["OnActionExecuted"]);
                Assert.IsTrue((bool)executor.HttpContext.Items["OnResultExecuting"]);
                Assert.IsTrue((bool)executor.HttpContext.Items["OnResultExecuted"]);
            }
        }
    }
}