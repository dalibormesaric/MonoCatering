using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Mono.Areas.Auction.Controllers
{
    public class Helper
    {
        public virtual string getCurrentUserID()
        {
            return HttpContext.Current.User.Identity.GetUserId();
        }

        public virtual bool isAjaxRequest()
        {
            return new HttpRequestWrapper(HttpContext.Current.Request).IsAjaxRequest();
        }
    }
}