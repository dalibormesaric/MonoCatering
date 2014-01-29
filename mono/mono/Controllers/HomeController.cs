using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using mono.Models;
using mono.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mono.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
            : this(new UserManager<MyUser>(new UserStore<MyUser>(new MonoDbContext())))
        {
        }

        public HomeController(UserManager<MyUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<MyUser> UserManager { get; private set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Restaurants()
        {
            //var Db = new IdentityDbContext();

            //var model = new List<RestaurantViewModel>();

            /*
            var resturantRole = from r in Db.Roles
                                where r.Name == "restaurant"
                                select r;

            var restaurants = Db.Users.Where(u => u.Roles.Select(r => r.Role).FirstOrDefault() == resturantRole.FirstOrDefault());
            
            foreach (var restaurant in restaurants)
            {
                var r = new RestaurantViewModel(restaurant);
                model.Add(r);
            }
             */

            var db = new MonoDbContext();

            return View(db.Restaurants.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
    }
}