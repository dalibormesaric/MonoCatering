using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mono.Models
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Image(this HtmlHelper helper, string imageName, object htmlAttributes)
        {
            var builder = new TagBuilder("img");

            if (String.IsNullOrEmpty(imageName))
            {
                builder.MergeAttribute("src", "/Content/Images/_.png");
            }
            else
            {
                builder.MergeAttribute("src", "/Content/Images/" + imageName + ".png");
            }

            builder.MergeAttribute("class", "thumbnail");
            builder.MergeAttribute("alt", imageName);
            builder.MergeAttribute("height", "140");
            builder.MergeAttribute("width", "140");
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

    }
}