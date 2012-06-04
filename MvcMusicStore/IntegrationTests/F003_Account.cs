using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NUnit.Framework;
using Verde.Executor;
using ScrapySharp;
using ScrapySharp.Extensions;
using HtmlAgilityPack;

namespace MvcMusicStore.IntegrationTests
{
    [TestFixture]
    public class F003_Account
    {
        [Test]
        public void Register_InvalidPassword_Fails()
        {
            var url = new UriBuilder(HttpContext.Current.Request.Url);
            url.Path = "Account/Register";
            url.Query = string.Empty;

            var formData = new NameValueCollection {
                {"UserName", "Bill"},
                {"Email", "bgates@microsoft.com"},
                {"Password", "password991"},
                {"ConfirmPassword", "password991"}
            };

            using (var executor = new MvcRequestExecutor(new RequestExecutorSettings(url.Uri) { Form = formData }))
            {
                executor.Execute();

                var responseText = executor.ResponseText;

                Assert.IsFalse(executor.ViewData.ModelState.IsValid);

                var errors = new List<string>();
                foreach (var modelState in executor.ViewData.ModelState.Values)
                    errors.AddRange(modelState.Errors.Select<ModelError, string>(e => e.ErrorMessage));

                Assert.AreEqual(1, errors.Count);

                string passwordError = "The password provided is invalid. Please enter a valid password value.";
                Assert.AreEqual(passwordError, errors[0]);

                var html = new HtmlDocument();
                html.LoadHtml(executor.ResponseText);
                var errorMessages = html.DocumentNode.CssSelect("div.validation-summary-errors ul>li");
                Assert.AreEqual(1, errorMessages.Count());
                Assert.AreEqual(passwordError, errorMessages.ElementAt(0).InnerText);
            }
        }

        [Test]
        public void Register_ValidInput_Redirects()
        {
            var url = new UriBuilder(HttpContext.Current.Request.Url);
            url.Path = "Account/Register";
            url.Query = string.Empty;

            var formData = new NameValueCollection {
                {"UserName", "ChrisColumbus"},
                {"Email", "chrisc@genoa.es"},
                {"Password", "explore_1492"},
                {"ConfirmPassword", "explore_1492"}
            };

            try
            {
                using (var executor = new MvcRequestExecutor(new RequestExecutorSettings(url.Uri) { Form = formData }))                
                { 
                    executor.Execute();

                    Assert.IsTrue(executor.ViewData.ModelState.IsValid);
                    Assert.AreEqual(302, executor.HttpContext.Response.StatusCode);
                    Assert.AreEqual("/", executor.HttpContext.Response.RedirectLocation);
                }
            }
            finally
            {
                Membership.DeleteUser(formData["UserName"]);
            }
         }
    }
}