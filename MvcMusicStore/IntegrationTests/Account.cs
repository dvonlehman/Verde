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
    public class Account
    {
        [Test]
        public void Register_InvalidPassword_Fails()
        {
            string invalidPassword = "1234";
            var formData = new NameValueCollection {
                {"UserName", "Bill"},
                {"Email", "bgates@microsoft.com"},
                {"Password", invalidPassword},
                {"ConfirmPassword", invalidPassword}
            };
                        
            using (var scope = new MvcExecutorScope(new ExecutorSettings("Account/Register") { Form = formData }))
            {
                var responseText = scope.ResponseText;

                Assert.IsFalse(scope.ViewData.ModelState.IsValid);

                var errors = new List<string>();
                foreach (var modelState in scope.ViewData.ModelState.Values)
                    errors.AddRange(modelState.Errors.Select<ModelError, string>(e => e.ErrorMessage));

                Assert.AreEqual(1, errors.Count);

                string passwordError = "The Password must be at least 6 characters long.";
                Assert.AreEqual(passwordError, errors[0]);

                var html = new HtmlDocument();
                html.LoadHtml(scope.ResponseText);

                var passwordErrorSpan = html.DocumentNode.CssSelect("span.field-validation-error");
                Assert.AreEqual(1, passwordErrorSpan.Count());
                Assert.AreEqual(passwordError, passwordErrorSpan.ElementAt(0).InnerText);
            }
        }

        [Test]
        public void Register_ValidInput_Redirects()
        {
            var formData = new NameValueCollection {
                {"UserName", "ChrisColumbus"},
                {"Email", "chrisc@genoa.es"},
                {"Password", "explore_1492"},
                {"ConfirmPassword", "explore_1492"}
            };

            try
            {
                using (var scope = new MvcExecutorScope(new ExecutorSettings("Account/Register") { Form = formData }))                
                { 
                    Assert.IsTrue(scope.ViewData.ModelState.IsValid);
                    Assert.AreEqual(302, scope.HttpContext.Response.StatusCode);
                    Assert.AreEqual("/", scope.HttpContext.Response.RedirectLocation);
                }
            }
            finally
            {
                // Cleanup
                Membership.DeleteUser(formData["UserName"]);
            }
         }
    }
}