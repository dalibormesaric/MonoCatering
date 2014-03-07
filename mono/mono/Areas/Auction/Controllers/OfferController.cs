using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mono.Areas.Auction.Controllers
{
    [Authorize(Roles = "restaurant")]
    public class OfferController : Controller
    {
        //
        // GET: /Auction/Offer/
        public ActionResult Index()
        {
            return View();
        }
	}
}