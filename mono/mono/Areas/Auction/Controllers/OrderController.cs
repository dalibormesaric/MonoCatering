using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mono.Models;
using mono.Areas.Auction.Models;
using mono.DAL;
using Microsoft.AspNet.Identity;

namespace mono.Areas.Auction.Controllers
{
    [Authorize(Roles = "user")]
    public class OrderController : Controller
    {
        private UnitOfWork unitOfWork;

        public OrderController()
        {
            unitOfWork = new UnitOfWork();
        }

        public OrderController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        // GET: /Auction/Order/
        public ActionResult Index()
        {
            var orders = unitOfWork.OrderRepository.Get().OrderByDescending(o => o.DateTime).ToList();
            return View("Index", orders);
        }

        // GET: /Auction/Order/Deactivate/5
        public ActionResult Deactivate(int? id)
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

            var userID = User.Identity.GetUserId();
            if (order.UserID != userID || order.Status != Status.Active)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                order.Status = Status.Expired;

                unitOfWork.OrderRepository.Update(order);
                unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
            }

            ViewBag.Message = "Unable to deactivate order. Try again, and if the problem persists contact your system administrator.";
            return View("Deactivate");
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
            return View(order);
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

            return View(orderUpdate);
        }

        // GET: /Auction/Order/Create
        public ActionResult Create()
        {
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
                    ModelState.AddModelError(string.Empty, "No items in basket.");
                    return View("Create");
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
                return RedirectToAction("Index");
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("Create");
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
