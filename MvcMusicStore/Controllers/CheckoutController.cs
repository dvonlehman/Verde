using System;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : AsyncController
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();
        const string PromoCode = "FREE";

        //
        // GET: /Checkout/AddressAndPayment

        public ActionResult AddressAndPayment()
        {
            return View();
        }

        //
        // POST: /Checkout/AddressAndPayment

        [HttpPost]
        public void AddressAndPaymentAsync(FormCollection values)
        {
            AsyncManager.OutstandingOperations.Increment();
            var worker = new BackgroundWorker();
            var order = new Order();
            bool success = true;

            worker.DoWork += (sender, e) =>
            {
                TryUpdateModel(order);

                string promoCode = values["PromoCode"];
                if (!String.IsNullOrEmpty(promoCode) && !string.Equals(promoCode, PromoCode, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "Promo code is not valid.");
                    success = false;
                    return;
                }

                try
                {
                    order.Username = User.Identity.Name;
                    order.OrderDate = DateTime.Now;

                    //Save Order
                    storeDB.Orders.Add(order);
                    storeDB.SaveChanges();

                    //Process the order
                    var cart = ShoppingCart.GetCart(this.HttpContext);
                    cart.CreateOrder(order);               
                    success = true;
                }
                catch
                {
                    success = false;
                }
            };

            worker.RunWorkerCompleted += (o, e) =>
            {
                AsyncManager.Parameters["order"] = order;
                AsyncManager.Parameters["success"] = success;
                AsyncManager.OutstandingOperations.Decrement();
            };

            worker.RunWorkerAsync(null);
        }

        public ActionResult AddressAndPaymentCompleted(Order order, bool success)
        {
            if (success)
                return RedirectToAction("Complete",
                   new { id = order.OrderId });

            //Invalid - redisplay with errors
            return View(order);           
        }

        //
        // GET: /Checkout/Complete

        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = storeDB.Orders.Any(
                o => o.OrderId == id &&
                o.Username == User.Identity.Name);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
