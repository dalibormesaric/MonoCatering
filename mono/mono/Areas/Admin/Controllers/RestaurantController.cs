using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mono.Models;
using mono.DAL;
using PagedList;

namespace mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class RestaurantController : Controller
    {
        //private UnitOfWork unitOfWork = new UnitOfWork();
        
        private UnitOfWork unitOfWork;

        public RestaurantController()
        {
            unitOfWork = new UnitOfWork();
        }

        public RestaurantController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /AdminRestaurant/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var restaurants = unitOfWork.RestaurantRepository.Get();

            if (!String.IsNullOrEmpty(searchString))
            {
                restaurants = restaurants.Where(r =>
                    r.Name.ToUpper().Contains(searchString.ToUpper()) ||
                    r.Description.ToUpper().Contains(searchString.ToUpper())
                );
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    restaurants = restaurants.OrderByDescending(r => r.Name);
                    break;
                default:
                    restaurants = restaurants.OrderBy(r => r.Name);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(restaurants.ToPagedList(pageNumber, pageSize));
        }

        // GET: /AdminCategory/Employers/5
        public ActionResult Employers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID((int)id);

            var users = unitOfWork.UserRepository.Get(filter: u => u.RestaurantID == (int)id);

            if (restaurant == null)
            {
                return HttpNotFound();
            }

            ViewBag.Restaurant = restaurant.Name;

            return View(users.ToList());
        }

        // GET: /AdminRestaurant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID((int)id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        // GET: /AdminRestaurant/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AdminRestaurant/Create
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

            return View(restaurant);
        }

        // GET: /AdminRestaurant/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID((int)id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        // POST: /AdminRestaurant/Edit/5
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
            return View(restaurant);
        }

        // GET: /AdminRestaurant/Delete/5
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
            Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID((int)id);
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(restaurant);
        }

        // POST: /AdminRestaurant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Restaurant restaurant = unitOfWork.RestaurantRepository.GetByID((int)id); ;
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
