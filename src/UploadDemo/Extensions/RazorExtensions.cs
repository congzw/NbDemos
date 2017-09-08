using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace UploadDemo.Extensions
{
    public static class RazorExtensions
    {
        public static IHtmlString VueTemplate(this HtmlHelper htmlHelper, string htmlContent, bool replaceSingleQuotes = true)
        {
            return htmlContent.ToVueTemplate(replaceSingleQuotes);
        }

        public static IHtmlString ToVueTemplate(this string value, bool replaceSingleQuotes = true)
        {
            #region demos
            //template: '\
            //    <span>\
            //      $\
            //      <input\
            //        ref="input"\
            //        v-bind:value="value"\
            //        v-on:input="updateValue($event.target.value)"\
            //      >\
            //    </span>\
            //  ',@Html.Partial("_Accordion")

            //1 replace ' with "
            //2 append line with \
            #endregion

            if (string.IsNullOrWhiteSpace(value))
            {
                return new HtmlString(string.Empty);
            }

            var stringBuilder = new StringBuilder();
            using (var reader = new StringReader(value))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    if (replaceSingleQuotes)
                    {
                        stringBuilder.AppendLine(line.Replace('\'', '"') + '\\');
                    }
                    else
                    {
                        stringBuilder.AppendLine(line + '\\');
                    }
                }
            }
            return new HtmlString(stringBuilder.ToString());
        }

        public static IHtmlString ScriptTag(this HtmlHelper htmlHelper, string url, bool render = true)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return ScriptTag(urlHelper, url, render);
        }

        public static IHtmlString ScriptTag(this UrlHelper urlHelper, string url, bool render = true)
        {
            if (!render || string.IsNullOrWhiteSpace(url))
            {
                return new HtmlString(string.Empty);
            }

            var script = new TagBuilder("script");
            script.Attributes["type"] = "text/javascript";
            script.Attributes["src"] = urlHelper.Content(ProcessUrl(url));
            return new HtmlString(script.ToString(TagRenderMode.Normal));
        }

        public static IHtmlString CssTag(this HtmlHelper htmlHelper, string url, bool render = true)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return CssTag(urlHelper, url, render);
        }

        public static IHtmlString CssTag(this UrlHelper urlHelper, string url, bool render = true)
        {
            if (!render || string.IsNullOrWhiteSpace(url))
            {
                return new HtmlString(string.Empty);
            }

            var script = new TagBuilder("link");
            script.Attributes["rel"] = "stylesheet";
            script.Attributes["href"] = urlHelper.Content(ProcessUrl(url));
            return new HtmlString(script.ToString(TagRenderMode.SelfClosing));
        }

        public static HelperResult List<T>(this IEnumerable<T> items, Func<T, HelperResult> template)
        {
            return new HelperResult(writer =>
            {
                foreach (var item in items)
                {
                    template(item).WriteTo(writer);
                }
            });
        }

        private static string ProcessUrl(string url)
        {
            if (url.StartsWith("~"))
            {
                return url;
            }
            if (url.StartsWith("/"))
            {
                return "~" + url;
            }
            return url;
        }
    }
}
