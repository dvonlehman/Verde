using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Models;
using NUnit.Framework;
using Verde;
using Verde.Executor;

namespace MvcMusicStore.IntegrationTests
{
    [IntegrationFixture(Sequence=50)]
    public class Store
    {
        [IntegrationTest]
        public void ControllerEvents_DoExecute()
        {
            using (var scope = new MvcExecutorScope("Store"))
            {
                var model = scope.ViewData.Model as IList<Genre>;
                Assert.IsNotNull(model);

                Assert.IsTrue((bool)scope.HttpContext.Items["OnActionExecuting"]);
                Assert.IsTrue((bool)scope.HttpContext.Items["OnActionExecuted"]);
                Assert.IsTrue((bool)scope.HttpContext.Items["OnResultExecuting"]);
                Assert.IsTrue((bool)scope.HttpContext.Items["OnResultExecuted"]);
            }
        }
    }
}