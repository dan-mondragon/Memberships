using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Memberships.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString GlyphLink(this HtmlHelper htmlHelper, string controller, string action, string text, string glyphicon, string cssClasses = "", string id = "")
        {
            var glyph = string.Format("<span class='glyphicon glyphicon-{0}'></span>", glyphicon);
            var anchor = new TagBuilder("a");
            anchor.MergeAttribute("href", string.Format("/{0}/{1}/", controller, action));
            anchor.InnerHtml = string.Format("{0} {1}", glyph, text);
            anchor.AddCssClass(cssClasses);
            anchor.GenerateId(id);

            return MvcHtmlString.Create(anchor.ToString(TagRenderMode.Normal));
        }
    }
}