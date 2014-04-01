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
        private IUnitOfWork unitOfWork;

        public OfferController(IUnitOfWork unitOfWork)
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

        public ActionResult Order(int id)
        {
            if (Request.IsAjaxRequest())
            {
                var order = unitOfWork.OrderRepository.GetByID(id);
                return PartialView("_Order", order);
            }
            else
                return RedirectToAction("Orders"); 
        }

        public int OrdersCount()
        {
            return unitOfWork.OrderRepository.Get(o => o.Status == Mono.Model.Status.Active, q => q.OrderByDescending(o => o.DateTime)).Count();
        }

        //
        // GET: /Auction/Offer/Details
        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }
            var order = unitOfWork.OrderRepository.GetByID(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View("Details", order);
        }

        // GET: /Auction/Offer/MakeOffer/3
        public ActionResult MakeOffer(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }
            var order = unitOfWork.OrderRepository.GetByID(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            if (order.Status != Status.Active)
            {
                return RedirectToAction("Orders");
            }

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
            try
            {
                if (ModelState.IsValid)
                {
                    if (unitOfWork.OrderRepository.GetByID(offer.OrderID).Status != Status.Active)
                    {
                        ModelState.AddModelError(string.Empty, "Order no longer active.");
                    }
                    else
                    {
                        offer.DateTime = System.DateTime.Now;
                        offer.RestaurantID = (int)unitOfWork.UserRepository.GetByID(User.Identity.GetUserId()).RestaurantID;

                        unitOfWork.OfferRepository.Insert(offer);
                        unitOfWork.Save();

                        var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                        hubContext.Clients.User(offer.Order.User.UserName).offersCountForOrderNew(offer.OrderID, offer.ID);

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            ViewBag.OrderID = offer.OrderID;
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
            if (offer == null)
            {
                return HttpNotFound();
            }
            if (offer.RestaurantID != unitOfWork.UserRepository.GetByID(User.Identity.GetUserId()).RestaurantID || offer.ID == offer.Order.AcceptedOfferID)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                if (offer.RestaurantID == unitOfWork.UserRepository.GetByID(User.Identity.GetUserId()).RestaurantID && offer.ID != offer.Order.AcceptedOfferID)
                {
                    var userName = offer.Order.User.UserName;
                    var orderID = offer.OrderID;

                    unitOfWork.OfferRepository.Delete(id);
                    unitOfWork.Save();

                    var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hubContext.Clients.User(userName).offersCountForOrderDeleted(orderID, offer.ID);

                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
            }
            return RedirectToAction("Delete", new { id = id, saveChangesError = true });
        }
	}
}