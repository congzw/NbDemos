using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace UploadDemo.Extensions
{
    public static class ResourceType
    {
        public const string Css = "css";
        public const string Js = "js";
    }

    public static class HtmlExtensions
    {
        public static IHtmlString Resource(this HtmlHelper htmlHelper, Func<object, dynamic> template, string type)
        {
            if (htmlHelper.ViewContext.HttpContext.Items[type] != null) ((List<Func<object, dynamic>>)htmlHelper.ViewContext.HttpContext.Items[type]).Add(template);
            else htmlHelper.ViewContext.HttpContext.Items[type] = new List<Func<object, dynamic>>() { template };

            return new HtmlString(String.Empty);
        }

        public static IHtmlString RenderResources(this HtmlHelper htmlHelper, string type)
        {
            if (htmlHelper.ViewContext.HttpContext.Items[type] != null)
            {
                List<Func<object, dynamic>> resources = (List<Func<object, dynamic>>)htmlHelper.ViewContext.HttpContext.Items[type];

                foreach (var resource in resources)
                {
                    if (resource != null) htmlHelper.ViewContext.Writer.Write(resource(null));
                }
            }

            return new HtmlString(String.Empty);
        }

        public static Func<object, dynamic> ScriptTag(this HtmlHelper htmlHelper, string url)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var script = new TagBuilder("script");
            script.Attributes["type"] = "text/javascript";
            script.Attributes["src"] = urlHelper.Content("~/" + url);
            return x => new HtmlString(script.ToString(TagRenderMode.Normal));
        }
    }
}
