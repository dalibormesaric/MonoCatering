using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mono.Model;
using Mono.Models;
using Mono.Data;
using PagedList;
using AutoMapper;
using System.Linq.Expressions;
using Mono.Helper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private IUnitOfWork unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: /Admin/User/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int pageNumber = ControllerHelper.newSearchPageNumber(ref searchString, page, currentFilter);   

            Expression<Func<MyUser, bool>> filter = null;
            if (!String.IsNullOrEmpty(searchString))
            {
                filter = (u =>
                    u.UserName.ToUpper().Contains(searchString.ToUpper()) ||
                    u.FirstName.ToUpper().Contains(searchString.ToUpper()) ||
                    u.LastName.ToUpper().Contains(searchString.ToUpper())
                );
            }

            Func<IQueryable<MyUser>, IOrderedQueryable<MyUser>> orderBy = null;
            switch (sortOrder)
            {
                case "Name_desc":
                    orderBy = (q => q.OrderByDescending(u => u.UserName));
                    break;
                case "FirstName":
                    orderBy = (q => q.OrderBy(u => u.FirstName));
                    break;
                case "FirstName_desc":
                    orderBy = (q => q.OrderByDescending(u => u.FirstName));
                    break;
                case "LastName":
                    orderBy = (q => q.OrderBy(u => u.LastName));
                    break;
                case "LastName_desc":
                    orderBy = (q => q.OrderByDescending(u => u.LastName));
                    break;
                default:
                    orderBy = (q => q.OrderBy(r => r.UserName));
                    break;
            }

            var users = unitOfWork.UserRepository.Get(filter: filter, orderBy: orderBy);

            Mapper.CreateMap<MyUser, AdminUserViewModel>().ForMember(dest => dest.Restaurant, conf => conf.MapFrom(ol => ol.Restaurant.Name));
            Mapper.CreateMap<MyUser, AdminUserViewModel>().ForMember(dest => dest.IsAdmin, conf => conf.MapFrom(ol => unitOfWork.IsAdmin(ol)));

            IEnumerable<AdminUserViewModel> model = Mapper.Map<IEnumerable<MyUser>, IEnumerable<AdminUserViewModel>>(users.ToList());

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "LastName_desc" : "LastName";

            return View("Index", model.ToPagedList(pageNumber, int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"].ToString()) ));
        }

        // GET: /Admin/User/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyUser user = unitOfWork.UserRepository.GetByID(id);
            if (user == null || unitOfWork.IsAdmin(user))
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

        // POST: /Admin/User/Edit/5
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

            if (user == null || unitOfWork.IsAdmin(user))
            {
                return RedirectToAction("Index");
            }
            try
            {
                user.RestaurantID = restaurantID;
                unitOfWork.UserRepository.Update(user);
                unitOfWork.Save();

                UserManager<MyUser> UserManager = new UserManager<MyUser>(new UserStore<MyUser>(new MonoDbContext()));

                if (restaurantID == null)
                {
                    UserManager.AddToRole(id, "user");
                    UserManager.RemoveFromRole(id, "restaurant");
                }
                else
                {
                    UserManager.AddToRole(id, "restaurant");
                    UserManager.RemoveFromRole(id, "user");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                return RedirectToAction("Edit", new { id = id, saveChangesError = true });
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
