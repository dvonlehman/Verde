using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using MvcMusicStore.Models;

namespace MvcMusicStore.IntegrationTests
{
    internal static class TestUtil
    {
        /// <summary>
        /// Add some albums to the specified user's cart.
        /// </summary>
        public static MvcMusicStore.Models.ShoppingCart AddItemsToCart(string userName, IEnumerable<Album> albums)
        {
            var cart = new MvcMusicStore.Models.ShoppingCart { ShoppingCartId = userName };
            cart.EmptyCart();
            foreach (var album in albums)
                cart.AddToCart(album);
            return cart;
        }

        public static IPrincipal CreateUser(string userName)
        {
            return new GenericPrincipal(new GenericIdentity(userName), null);
        }
    }
}