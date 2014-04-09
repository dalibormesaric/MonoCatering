using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mono.Helper
{
    public static class ControllerHelper
    {
        public static int newSearchPageNumber(ref string searchString, int? page, string currentFilter)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            return page ?? 1;
        }

    }
}