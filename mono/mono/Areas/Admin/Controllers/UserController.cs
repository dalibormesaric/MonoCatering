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
using AutoMapper;

namespace mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private UnitOfWork unitOfWork;

        public UserController()
        {
            unitOfWork = new UnitOfWork();
        }

        public UserController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /AdminUser/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "LastName_desc" : "LastName";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var users = unitOfWork.UserRepository.Get();

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.UserName.ToUpper().Contains(searchString.ToUpper()) ||
                    u.FirstName.ToUpper().Contains(searchString.ToUpper()) ||
                    u.LastName.ToUpper().Contains(searchString.ToUpper())
                );
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    users = users.OrderByDescending(u => u.UserName);
                    break;
                case "FirstName":
                    users = users.OrderBy(u => u.FirstName);
                    break;
                case "FirstName_desc":
                    users = users.OrderByDescending(u => u.FirstName);
                    break;
                case "LastName":
                    users = users.OrderBy(u => u.LastName);
                    break;
                case "LastName_desc":
                    users = users.OrderByDescending(u => u.LastName);
                    break;
                default:
                    users = users.OrderBy(r => r.UserName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            Mapper.CreateMap<MyUser, AdminUserViewModel>().ForMember(dest => dest.Restaurant, conf => conf.MapFrom(ol => ol.Restaurant.Name));
            IEnumerable<AdminUserViewModel> model = Mapper.Map<IEnumerable<MyUser>, IEnumerable<AdminUserViewModel>>(users.ToList());

            return View("Index", model.ToPagedList(pageNumber, pageSize));
        }

        // GET: /AdminUser/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyUser user = unitOfWork.UserRepository.GetByID(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.RestaurantID = new SelectList(unitOfWork.RestaurantRepository.Get(orderBy: q => q.OrderBy(r => r.Name)), "ID", "Name", user.RestaurantID);
            return View("Edit", user);
        }

        public class EditUserRestaurant
        {
            public string ID;
            public int RestaurantID;
        }

        // POST: /AdminUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, int? restaurantID)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            MyUser user = unitOfWork.UserRepository.GetByID(id);
            
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            try
            {
                user.RestaurantID = restaurantID;
                unitOfWork.UserRepository.Update(user);
                unitOfWork.Save();
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Edit", new { id = id, saveChangesError = true });
            }
            //todo add role restaurant to user or remove if restaurantID == null

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
