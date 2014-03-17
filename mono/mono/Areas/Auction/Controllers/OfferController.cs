using Mono.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Mono.Model;
using System.Data;
using System.Net;

namespace Mono.Areas.Auction.Controllers
{
    [Authorize(Roles = "restaurant")]
    public class OfferController : Controller
    {
        private UnitOfWork unitOfWork;

        public OfferController()
        {
            unitOfWork = new UnitOfWork();
        }

        public OfferController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //
        // GET: /Auction/Offer/
        public ActionResult Index()
        {
            var restaurantID = unitOfWork.UserRepository.GetByID(User.Identity.GetUserId()).RestaurantID;
            var offers = unitOfWork.OfferRepository.Get(o => o.RestaurantID == restaurantID, q => q.OrderByDescending(o => o.DateTime)).ToList();
            return View("Index", offers);
        }

        // GET: /Auction/Offer/orders
        public ActionResult Orders()
        {
            var orders = unitOfWork.OrderRepository.Get(o => o.Status == Mono.Model.Status.Active, q => q.OrderByDescending(o => o.DateTime)).ToList();
            return View("Orders", orders);
        }

        //
        // GET: /Auction/Offer/Details
        public ActionResult Details(int id)
        {
            var foodIngredients = unitOfWork.FoodIngredientRepository.Get(fi => fi.OrderID == id).ToList();

            if (foodIngredients.Count == 0)
                return HttpNotFound();

            var order = unitOfWork.OrderRepository.GetByID(id);

            ViewBag.Description = order.Description;
            ViewBag.Address = order.User.Address;
            ViewBag.OrderID = order.ID;

            return View("Details", foodIngredients);
        }

        // GET: /Auction/Offer/MakeOffer/3
        public ActionResult MakeOffer(int id)
        {
            ViewBag.OrderID = id;

            return View("MakeOffer");
        }

        // POST: /Auction/Offer/MakeOffer
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeOffer([Bind(Include = "ID,Description,Price,DeliveryTime,OrderID")] Offer offer)
        {
            ViewBag.OrderID = offer.OrderID;

            try
            {
                if (ModelState.IsValid)
                {
                    offer.DateTime = System.DateTime.Now;
                    offer.RestaurantID = (int)unitOfWork.UserRepository.GetByID(User.Identity.GetUserId()).RestaurantID;

                    var order = unitOfWork.OrderRepository.GetByID(offer.OrderID);

                    if (order == null || order.Status != Status.Active)
                    {
                        ModelState.AddModelError(string.Empty, "Order no longer active.");
                        return View("MakeOffer", offer);
                    }

                    unitOfWork.OfferRepository.Insert(offer);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("MakeOffer", offer);
        }

        // GET: /Auction/Offer/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Offer offer = unitOfWork.OfferRepository.GetByID((int)id);
            if (offer == null || offer.AcceptedOrderID != null)
            {
                return HttpNotFound();
            }
            return View("Delete", offer);
        }

        // POST: /Auction/Offer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Offer offer = unitOfWork.OfferRepository.GetByID(id);
                if (offer.AcceptedOrderID != null)
                {
                    return HttpNotFound();
                }
                unitOfWork.OfferRepository.Delete(id);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }
	}
}