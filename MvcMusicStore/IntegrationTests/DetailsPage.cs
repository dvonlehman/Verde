using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Controllers;
using MvcMusicStore.Models;
using NUnit.Framework;
using Verde.Executor;
using Verde;
using ScrapySharp;
using ScrapySharp.Extensions;
using HtmlAgilityPack;

namespace MvcMusicStore.IntegrationTests
{
    [IntegrationFixture(Sequence=5)]
    public class DetailsPage
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        [IntegrationTest]
        public void Index_Load_ExpectedHtml()
        {
            // Get a product to load the details page for.
            var album = storeDB.Albums
                .Take(1)
                .First();

            using (var scope = new MvcExecutorScope("Store/Details/" + album.AlbumId))
            {
                Assert.AreEqual(200, scope.HttpContext.Response.StatusCode);
                Assert.IsTrue(scope.Controller is StoreController);
                Assert.AreEqual("Details", scope.Action);

                var model = scope.Controller.ViewData.Model as Album;
                Assert.IsNotNull(model);
                Assert.AreEqual(album.AlbumId, model.AlbumId);

                Assert.IsFalse(String.IsNullOrEmpty(scope.ResponseText));

                // Load the ResponseText into an HtmlDocument
                var html = new HtmlDocument();
                html.LoadHtml(scope.ResponseText);

                // Use ScrappySharp CSS selector to make assertions about the rendered HTML
                Assert.AreEqual(album.Title, html.DocumentNode.CssSelect("#main h2").First().InnerText);
            }
        }
    }
}