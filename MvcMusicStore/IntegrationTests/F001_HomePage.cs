using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Models;
using MvcMusicStore.Controllers;
using NUnit.Framework;
using Verde.Executor;
using ScrapySharp;
using ScrapySharp.Extensions;
using HtmlAgilityPack;

namespace MvcMusicStore.IntegrationTests
{
    [TestFixture]
    public class F001_HomePage
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        [Test]
        public void Index_Load_ExpectedHtml()
        {
            using (var executor = new MvcRequestExecutorContext(new RequestExecutorSettings(string.Empty)))
            {
                var controller = executor.Controller as HomeController;
                Assert.IsNotNull(controller);
                Assert.AreEqual(executor.Action, "Index");

                var model = controller.ViewData.Model as List<MvcMusicStore.Models.Album>;
                Assert.IsNotNull(model, "Expected the Model to be a list of Albums.");

                var html = new HtmlDocument();
                html.LoadHtml(executor.ResponseText);

                AssertCategoryMenuValid(html);
                AssertAlbumListValid(html);
            }
        }

        // Verify the output of the categories navigation menu.  There should 
        // be a link corresponding to each genre.
        private void AssertCategoryMenuValid(HtmlDocument html)
        {
            var categoriesList = html.DocumentNode.CssSelect("#categories");
            Assert.IsNotNull(categoriesList, "Could not find the #categories node.");

            var categoryLinks = categoriesList.CssSelect("li>a");
            Assert.AreEqual(storeDB.Genres.Count(), categoryLinks.Count(),
                "Did not find the expected number of category links in the navigation menu.");

            foreach (var genre in storeDB.Genres)
            {
                Assert.IsTrue(categoryLinks.Any(node => node.InnerText == genre.Name),
                    "No link in the category nav for genre " + genre.Name);
            }
        }

        private void AssertAlbumListValid(HtmlDocument html)
        {
            var albumList = html.DocumentNode.CssSelect("#album-list").FirstOrDefault();
            Assert.IsNotNull(albumList, "Could not find the #album-list node.");

            var albumLinks = albumList.CssSelect("li>a");
            Assert.AreEqual(5, albumLinks.Count(), "Expected 5 featured album links");
        }
    }
}