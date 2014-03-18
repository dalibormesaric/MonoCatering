using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mono.Model;
using Mono.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Linq.Expressions;

namespace Mono.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            unitOfWork = new UnitOfWork();
        }

        public HomeController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private UnitOfWork unitOfWork;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Restaurants(string sortOrder, string currentFilter, string searchString, int? page)
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
            int pageNumber = (page ?? 1);

            return View("Restaurants", restaurants.ToPagedList(pageNumber, Global.PageSize));
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = "user")]
        public ActionResult ChatUser()
        {
            return View();
        }

        [Authorize(Roles = "restaurant")]
        public ActionResult ChatRestaurant()
        {
            return View();
        }

    }
}