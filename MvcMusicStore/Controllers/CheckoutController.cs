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
        const string PromoCode = "FREE";

        private readonly IMusicStoreEntities _entities;

        public CheckoutController(IMusicStoreEntities entities)
        {
            _entities = entities;
        }

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

            TryUpdateModel(order);

            string promoCode = values["PromoCode"];
            if (!String.IsNullOrEmpty(promoCode) && !string.Equals(promoCode, PromoCode, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Promo code is not valid.");
                success = false;
            }

            string userName = User.Identity.Name;
            var cart = ShoppingCart.GetCart(this.HttpContext);

            worker.DoWork += (sender, args) =>
            {
                if (!success)
                    return;

                try
                {
                    order.Username = userName;
                    order.OrderDate = DateTime.Now;

                    //Save Order
                    _entities.Orders.Add(order);
                    _entities.SaveChanges();

                    //Process the order
                    cart.CreateOrder(order);               
                    success = true;
                }
                catch (Exception e)
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
            bool isValid = _entities.Orders.Any(
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
