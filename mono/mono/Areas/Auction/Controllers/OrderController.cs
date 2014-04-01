using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mono.Model;
using Mono.Areas.Auction.Models;
using Mono.Data;
using Microsoft.AspNet.Identity;

namespace Mono.Areas.Auction.Controllers
{
    [Authorize(Roles = "user")]
    public class OrderController : Controller
    {
        private IUnitOfWork unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /Auction/Order/
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            var orders = unitOfWork.OrderRepository.Get(o => o.UserID == userID, q => q.OrderByDescending(o => o.DateTime), "Offers").ToList();
            return View("Index", orders);
        }

        public int OffersCount()
        {
            var userID = User.Identity.GetUserId();
            return unitOfWork.OrderRepository.Get(o => o.UserID == userID && o.Status == Status.Active, q => q.OrderByDescending(o => o.DateTime), "Offers").Select(o => o.Offers.Count).Sum();
        }

        public ActionResult OffersCountForOrder(int id)
        {
            if(Request.IsAjaxRequest())
            {
                var userID = User.Identity.GetUserId();
                var order = unitOfWork.OrderRepository.GetByID(id);

                if (order.UserID == userID)
                    return PartialView("_OffersCountForOrder", new Tuple<int, int>(order.Offers.Count, order.ID));
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: /Auction/Order/Deactivate/5
        public ActionResult Deactivate(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Unable to deactivate order. Try again, and if the problem persists contact your system administrator.";
            }
            Order order = unitOfWork.OrderRepository.GetByID(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (order.UserID != User.Identity.GetUserId() || order.Status != Status.Active)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
          
            return View("Deactivate", order);
        }

        // POST: /Auction/Order/Deactivate/5
        [HttpPost, ActionName("Deactivate")]
        [ValidateAntiForgeryToken]
        public ActionResult DeactivateConfirmed(int id)
        {
            Order order = unitOfWork.OrderRepository.GetByID(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (order.UserID != User.Identity.GetUserId() || order.Status != Status.Active)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                order.Status = Status.Expired;

                unitOfWork.OrderRepository.Update(order);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Deactivate", new { id = id, saveChangesError = true });
            }

            var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.Group("restaurant").removeOrder(order.ID);

            return RedirectToAction("Index");
        }

        // GET: /Auction/Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = unitOfWork.OrderRepository.GetByID(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View("Edit", order);
        }

        // POST: /Auction/Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Description,DateTime,Status,UserID")] Order orderUpdate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Order order = unitOfWork.OrderRepository.GetByID(orderUpdate.ID);
                    if (order != null && order.UserID == User.Identity.GetUserId())
                    {
                        order.Description = orderUpdate.Description;

                        unitOfWork.OrderRepository.Update(order);
                        unitOfWork.Save();

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("Edit", orderUpdate);
        }

        // GET: /Auction/Order/Create
        public ActionResult Create()
        {
            var userID = User.Identity.GetUserId();
            var foodIngredients = unitOfWork.FoodIngredientRepository.Get(fi => fi.UserID == userID && fi.OrderID == null).ToList();

            if (foodIngredients.Count == 0)
            {
                return RedirectToAction("Index", "Basket");
            }

            return View("Create");
        }

        // POST: /Auction/Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string description)
        {
            try
            {
                var userID = User.Identity.GetUserId();
                var foodIngredients = unitOfWork.FoodIngredientRepository.Get(fi => fi.UserID == userID && fi.OrderID == null).ToList();

                if (foodIngredients.Count == 0)
                {
                    return RedirectToAction("Index", "Basket");
                }

                Order order = new Order
                {
                    Description = description,
                    DateTime = System.DateTime.Now,
                    Status = Status.Active,
                    UserID = userID
                };

                order.FoodIngredients = new List<FoodIngredient>();

                foreach (var foodIngredient in foodIngredients)
                {
                    order.FoodIngredients.Add(foodIngredient);
                }

                unitOfWork.OrderRepository.Insert(order);
                unitOfWork.Save();

                var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                hubContext.Clients.Group("restaurant").addOrder(order.ID);

                return RedirectToAction("Index");
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            
            return View("Create");
        }

        //
        // GET: /Auction/Order/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = unitOfWork.OrderRepository.GetByID(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            if (order.UserID != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Description = order.Description;
            return View("Details", order.FoodIngredients.ToList());
        }

        //
        // GET: /Auction/Order/Offers
        public ActionResult Offers(int? id)
        {
            var userID = User.Identity.GetUserId();
            var offers = unitOfWork.OfferRepository.Get(o => o.OrderID == id && o.Order.UserID == userID, q => q.OrderByDescending(o => o.DateTime)).ToList();

            return View("Offers", offers);
        }

        //
        // GET: /Auction/Order/Offer
        public ActionResult Offer(int id)
        {
            if (Request.IsAjaxRequest())
            {
                var userID = User.Identity.GetUserId();
                var offer = unitOfWork.OfferRepository.GetByID(id);

                return PartialView("_Offer", offer);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: /Auction/Order/Accept/5
        public ActionResult Accept(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Accept failed. Try again, and if the problem persists see your system administrator.";
            }
            Offer offer = unitOfWork.OfferRepository.GetByID((int)id);
            if (offer == null) 
            {
                return HttpNotFound();
            }
            if (offer.Order.UserID != User.Identity.GetUserId() || offer.Order.Status != Status.Active)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View("Accept", offer);
        }

        // POST: /Auction/Order/Accept/5
        [HttpPost, ActionName("Accept")]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptConfirmed(int id)
        {
            Offer offer = unitOfWork.OfferRepository.GetByID(id);
            if (offer == null)
            {
                return HttpNotFound();
            }
            if (offer.Order.UserID != User.Identity.GetUserId() || offer.Order.Status != Status.Active)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                offer.Order.Status = Status.Accepted;
                offer.Order.AcceptedOfferID = id;
                offer.Order.AcceptedDateTime = System.DateTime.Now;

                unitOfWork.OfferRepository.Update(offer);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Accept", new { id = id, saveChangesError = true });
            }
            
            var hubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.Group("restaurant").removeOrder(offer.OrderID);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
