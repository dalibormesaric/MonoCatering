using mono.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mono.Controllers
{
    public class AuctionController : Controller
    {
        [Authorize(Roles = "user")]
        public ActionResult Orders()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        public ActionResult newOrder(string id = null)
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newOrder(OrderViewModel model)
        {
            return View();
        }

        [Authorize(Roles = "user")]
        public ActionResult EditOrder(string id = null)
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder(OrderViewModel model)
        {
            return View();
        }

        [Authorize(Roles = "user")]
        public ActionResult DeleteOrder(string id)
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpPost, ActionName("deleteOrder")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrderConfirmed(string id)
        {
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "user")]
        public ActionResult acceptOffer(string id = null)
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult acceptOffer(OfferViewModel model)
        {
            return View();
        }

        [Authorize(Roles = "restaurant")]
        public ActionResult Offers()
        {
            return View();
        }

        [Authorize(Roles = "restaurant")]
        public ActionResult newOffer(string id = null)
        {
            return View();
        }

        [Authorize(Roles = "restaurant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult newOffer(OfferViewModel model)
        {
            return View();
        }

        [Authorize(Roles = "restaurant")]
        public ActionResult EditOffer(string id = null)
        {
            return View();
        }

        [Authorize(Roles = "restaurant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOffer(OfferViewModel model)
        {
            return View();
        }

        [Authorize(Roles = "restaurant")]
        public ActionResult DeleteOffer(string id)
        {
            return View();
        }

        [Authorize(Roles = "restaurant")]
        [HttpPost, ActionName("deleteOffer")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOfferConfirmed(string id)
        {
            return RedirectToAction("Index");
        }
	}
}