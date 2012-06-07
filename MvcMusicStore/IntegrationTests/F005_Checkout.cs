using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Verde.Executor;
using MvcMusicStore.Models;

namespace MvcMusicStore.IntegrationTests
{
    [TestFixture]
    public class F005_Checkout
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        [Test]
        public void Checkout_Success()
        {
            string userName = "MilesDavis";
            int numItemsInCart = 2;

            var cart = TestUtil.AddItemsToCart(userName, storeDB.Albums.Take(numItemsInCart));

            var form = new NameValueCollection
            {
             { "FirstName","Miles"},
             {"LastName","Davis"},
             {"Address","100 Broadway Ave."},
             {"City","New York"},
             {"State","NY"},
             {"PostalCode","12345"},
             {"Country","United States"},
             {"Phone","111-111-1111"},
             {"Email","miles@davis.sax"},
             {"PromoCode","FREE"}
            };

            var settings = new RequestExecutorSettings("Checkout/AddressAndPayment") { 
                Form=form, 
                HttpMethod="POST", 
                User=TestUtil.CreateUser(userName)
            };

            using (var executor = new MvcRequestExecutorContext(settings))
            {
                Assert.AreEqual(302, executor.HttpContext.Response.StatusCode);

                var match = new Regex("/Checkout/Complete/(\\d+)").Match(executor.HttpContext.Response.RedirectLocation);
                Assert.IsTrue(match.Success);

                var orderId = Int32.Parse(match.Groups[1].Value);
                
                var order = storeDB.Orders.Single(o=>o.OrderId==orderId);
                Assert.AreEqual(form["FirstName"], order.FirstName);
                Assert.AreEqual(form["Email"], order.Email);

                var orderDetails = storeDB.OrderDetails.Where(od => od.OrderId == orderId);
                Assert.AreEqual(numItemsInCart, orderDetails.Count());

                // Cleanup
                storeDB.Orders.Remove(order);
                storeDB.SaveChanges();
            }
        }
    }
}