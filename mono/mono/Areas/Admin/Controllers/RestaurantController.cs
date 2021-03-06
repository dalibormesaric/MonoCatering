﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mono.Model;
using Mono.Data;
using PagedList;
using System.Linq.Expressions;
using Mono.Helper;

namespace Mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class RestaurantController : Controller
    {      
        private IUnitOfWork unitOfWork;

        public RestaurantController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /Admin/Restaurant/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageNumber = ControllerHelper.newSearchPageNumber(ref searchString, page, currentFilter);

            Expression<Func<Restaurant, bool>> filter = null;
            if (!String.IsNullOrEmpty(searchString))
            {
                filter = (r =>
                    r.Name.ToUpper().Contains(searchString.ToUpper())
                );
            }

            Func<IQueryable<Restaurant>, IOrderedQueryable<Restaurant>> orderBy = null;
            switch (sortOrder)
            {
                case "Name_desc":
                    orderBy = (q => q.OrderByDescending(r => r.Name));
                    break;
                default:
                    orderBy = (q => q.OrderBy(r => r.Name));
                    break;
            }

            var restaurants = unitOfWork.RestaurantRepository.Get(filter: filter, orderBy: orderBy);

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

            return View("Index", restaurants.ToPagedList(pageNumber, int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"].ToString()) ));
        }

        // GET: /Admin/Category/Employers/5
        public ActionResult Employers(int id)
        {
            Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }

            var users = unitOfWork.UserRepository.Get(filter: u => u.RestaurantID == (int)id, orderBy: q => q.OrderBy(r => r.LastName + " " + r.LastName));

            ViewBag.Restaurant = restaurant.Name;

            return View("Employers", users.ToList());
        }

        // GET: /Admin/Restaurant/Details/5
        public ActionResult Details(int id)
        {
            Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }

            return View("Details", restaurant);
        }

        // GET: /Admin/Restaurant/Create
        public ActionResult Create()
        {
            return View("Create");
        }

        // POST: /Admin/Restaurant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Name,Description,Address,Phone,OIB")] Restaurant restaurant)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.RestaurantRepository.Insert(restaurant);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("Create", restaurant);
        }

        // GET: /Admin/Restaurant/Edit/5
        public ActionResult Edit(int id)
        {
            Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }

            return View("Edit", restaurant);
        }

        // POST: /Admin/Restaurant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,Name,Description,Address,Phone,OIB")] Restaurant restaurant)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.RestaurantRepository.Update(restaurant);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return View("Edit", restaurant);
        }

        // GET: /Admin/Restaurant/Delete/5
        public ActionResult Delete(int id, bool? saveChangesError = false)
        {
            Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID(id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            return View("Delete", restaurant);
        }

        // POST: /Admin/Restaurant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID(id); ;
                unitOfWork.RestaurantRepository.Delete(id);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            
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
