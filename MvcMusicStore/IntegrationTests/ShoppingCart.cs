using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using NUnit.Framework;
using Verde.Executor;
using ScrapySharp.Extensions;
using HtmlAgilityPack;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;
using Newtonsoft.Json;
using Verde;

namespace MvcMusicStore.IntegrationTests
{
	[IntegrationFixture(Sequence = 10)]
	public class ShoppingCart
	{
		private IMusicStoreEntities storeDB;


		public ShoppingCart(IMusicStoreEntities storeDB)
		{
			this.storeDB = storeDB;
		}

		[IntegrationTest]
		public void AddToCart_ValidItem_Succeeds()
		{
			// Get a product to load the details page for.
			var album = storeDB.Albums
				.Take(1)
				.First();

			var settings = new ExecutorSettings("ShoppingCart/AddToCart/" + album.AlbumId)
			{
				User = new GenericPrincipal(new GenericIdentity("GenghisKahn"), null)
			};

			using (var scope = new MvcExecutorScope(settings))
			{
				Assert.AreEqual(302, scope.HttpContext.Response.StatusCode);
				Assert.AreEqual("/ShoppingCart", scope.HttpContext.Response.RedirectLocation);

				// Now verify that the cart contains the item we just added.
				var cart = MvcMusicStore.Models.ShoppingCart.GetCart(scope.HttpContext);
				var cartItems = cart.GetCartItems();
				Assert.AreEqual(1, cartItems.Count);
				Assert.AreEqual(album.AlbumId, cartItems[0].AlbumId);

				// Finally clear the cart.
				cart.EmptyCart();
			}
		}

		[IntegrationTest]
		public void ViewCart_ExpectedHtml()
		{
			var albumsToCart = storeDB.Albums.Take(2);
			string userName = "DavidBowie";

			var cart = TestUtil.AddItemsToCart(userName, albumsToCart);

			var settings = new ExecutorSettings("ShoppingCart/Index")
			{
				User = TestUtil.CreateUser(userName)
			};

			using (var scope = new MvcExecutorScope(settings))
			{
				var viewModel = scope.ViewData.Model as ShoppingCartViewModel;
				Assert.IsNotNull(viewModel);
				Assert.AreEqual(albumsToCart.Count(), viewModel.CartItems.Count);
				foreach (var album in albumsToCart)
					Assert.IsTrue(viewModel.CartItems.Any(c => c.AlbumId == album.AlbumId));

				Assert.IsFalse(String.IsNullOrEmpty(scope.ResponseText));
			}

			// Cleanup
			cart.EmptyCart();
		}

		[IntegrationTest]
		public void RemoveFromCart_ValidJson()
		{
			// Add an item to the cart so we have something to remove.
			string userName = "JimmyHendrix";
			MvcMusicStore.Models.ShoppingCart cart = TestUtil.AddItemsToCart(
				userName, storeDB.Albums.Take(1));
			var recordId = cart.GetCartItems().First().RecordId;

			var settings = new ExecutorSettings("ShoppingCart/RemoveFromCart/" + recordId)
			{
				User = TestUtil.CreateUser(userName),
				HttpMethod = "POST"
			};

			using (var scope = new MvcExecutorScope(settings))
			{
				Assert.AreEqual("application/json", scope.HttpContext.Response.ContentType, "Expected json to be returned.");

				var deserializedResponse = JsonConvert.DeserializeObject<ShoppingCartRemoveViewModel>(scope.ResponseText);
				Assert.AreEqual(0.0d, deserializedResponse.CartTotal, "The shopping cart total should be $0.00.");
				Assert.AreEqual(0, deserializedResponse.ItemCount, "The shopping cart should have 0 items left.");
				Assert.AreEqual(recordId, deserializedResponse.DeleteId);
			}
		}
	}
}