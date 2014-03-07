using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mono
{
    public static class Global
    {
        private const int pageSize = 3;

        public static int PageSize
        {
            get
            {
                return pageSize;
            }
        }
    }
}