using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using mono.Models;
using mono.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace mono.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}