using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Models;
using NUnit.Framework;
using Verde.Executor;
using Verde;

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

            using (var executor = new MvcRequestExecutorContext("Store/Details/" + album.AlbumId))
            {
                Assert.IsFalse(String.IsNullOrEmpty(executor.ResponseText));
            }
        }
    }
}