using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Mono.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");
        }
	}
}