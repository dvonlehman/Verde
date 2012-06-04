﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Models;
using NUnit.Framework;
using Verde.Executor;

namespace MvcMusicStore.IntegrationTests
{
    [TestFixture]
    public class F002_DetailsPage
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        [Test]
        public void Index_Load_ExpectedHtml()
        {
            // Get a product to load the details page for.
            var album = storeDB.Albums
                .Take(1)
                .First();

            var url = new UriBuilder(HttpContext.Current.Request.Url);
            url.Query = string.Empty;
            url.Path = "Store/Details/" + album.AlbumId;

            using (var executor = new MvcRequestExecutor(url.Uri))
            {
                executor.Execute();
                Assert.IsFalse(String.IsNullOrEmpty(executor.ResponseText));
            }
        }
    }
}