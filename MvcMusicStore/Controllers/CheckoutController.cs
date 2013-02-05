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

        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values)
        {
            var order = new Order();
            TryUpdateModel(order);

            string promoCode = values["PromoCode"];
            if (!String.IsNullOrEmpty(promoCode) && !string.Equals(promoCode, PromoCode, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Promo code is not valid.");
                //Invalid - redisplay with errors
                return View(order);   
            }

            string userName = User.Identity.Name;
            var cart = ShoppingCart.GetCart(this.HttpContext);

            order.Username = userName;
            order.OrderDate = DateTime.Now;

            //Save Order
            _entities.Orders.Add(order);
            _entities.SaveChanges();

            //Process the order
            cart.CreateOrder(order);

            return RedirectToAction("Complete",
                   new { id = order.OrderId });
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
