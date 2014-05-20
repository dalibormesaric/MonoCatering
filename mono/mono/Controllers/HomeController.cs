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
using System.IO;
using System.Web.Helpers;
using System.Data;
using Mono.Models;

namespace Mono.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        

        public ActionResult Index()
        {
            var foods = unitOfWork.FoodIngredientRepository.Get(q => q.Order.Status == Status.Accepted, null, "Food").GroupBy(q => q.FoodID, q => new { q.Food.Name, q.Food.PhotoID }).OrderByDescending(q => q.Count()).Take(10).Select(q => new HomeViewModel { ID = q.Key, Name = q.First().Name, PhotoID = q.First().PhotoID, Count = q.Count() });

            return View(foods);
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

            return View("Restaurants", restaurants.ToPagedList(pageNumber, int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["PageSize"].ToString()) ));
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}