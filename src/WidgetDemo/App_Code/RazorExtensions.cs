using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace WidgetDemo
{
    public static class RazorExtensions
    {
        #region VueTemplate

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
            //  ',

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

        #endregion

        #region PartialIf

        //how to
        //@{ Html.RenderPartialIf("_Aside", Request.IsAuthenticated); }
        //@Html.PartialIf("_Aside", Request.IsAuthenticated)

        public static void RenderPartialIf(this HtmlHelper htmlHelper, string partialViewName, bool condition)
        {
            if (!condition)
            {
                return;
            }
            htmlHelper.RenderPartial(partialViewName);
        }
        public static void RenderPartialIf(this HtmlHelper htmlHelper, string partialViewName, Func<bool> conditionFunc)
        {
            if (conditionFunc == null || !conditionFunc.Invoke())
            {
                return;
            }
            htmlHelper.RenderPartial(partialViewName);
        }

        public static IHtmlString PartialIf(this HtmlHelper htmlHelper, string partialViewName, bool condition)
        {
            if (!condition)
            {
                return MvcHtmlString.Empty;
            }
            return htmlHelper.Partial(partialViewName);
        }

        public static IHtmlString PartialIf(this HtmlHelper htmlHelper, string partialViewName, Func<bool> conditionFunc)
        {
            if (conditionFunc == null || !conditionFunc.Invoke())
            {
                return MvcHtmlString.Empty;
            }
            return htmlHelper.Partial(partialViewName);
        }

        #endregion

        #region Sections

        /// <summary>
        /// 如果@section未定义，则渲染部分页
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="page"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IHtmlString RenderSectionOrPartial<TModel>(this WebViewPage<TModel> page, string name)
        {
            //Unify/_Breadcrumbs => Unify_Breadcrumbs
            var fixSectionName = name.Replace("/", "");
            var renderSection = page.RenderSection(fixSectionName, false);
            if (renderSection != null)
            {
                return renderSection;
            }
            return page.Html.Partial(name);
        }

        /// <summary>
        /// 用指定的部分页渲染@section
        /// </summary>
        /// <param name="page"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IHtmlString RenderSectionWithPartial(this WebViewPage page, string name)
        {
            //@section _Xxx
            //{
            //    @Html.Partial("_Xxx")
            //}
            var fixSectionName = name.Replace("/", "");
            page.DefineSection(fixSectionName, () =>
            {
                page.Html.RenderPartial(name);
            });
            return null;
        }

        /// <summary>
        /// 隐藏指定的@section
        /// </summary>
        /// <param name="page"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IHtmlString HideSection(this WebViewPage page, string name)
        {
            //@section _Xxx
            //{
            //
            //}
            var fixSectionName = name.Replace("/", "");
            page.DefineSection(fixSectionName, () =>
            {

            });
            return null;
        }

        #endregion

        #region Tags

        //how to
        //@*<link href="~/Content/css/bootstrap.css" rel="stylesheet"/>*@
        //@Html.CssTag("~/Content/css/bootstrap.css")
        //@Url.CssTag("~/Content/css/bootstrap.css")

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

        #endregion

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

        //@using System.Text;
        //@functions {
        //    public static IHtmlString Repeat(int times, Func<int, object> template) {
        //        StringBuilder builder = new StringBuilder();
        //        for(int i = 0; i < times; i++) {
        //            builder.Append(template(i));
        //        }
        //        return new HtmlString(builder.ToString());
        //    }
        //}

        //<!DOCTYPE html>
        //<html>
        //    <head>
        //        <title>Repeat Helper Demo</title>
        //    </head>
        //    <body>
        //        <p>Repeat Helper</p>
        //        <ul>
        //            @Repeat(10, @<li>List Item #@item</li>);
        //        </ul>
        //    </body>
        //</html>
        public static IHtmlString Repeat(int times, Func<int, object> template)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < times; i++)
            {
                builder.Append(template(i));
            }
            return new HtmlString(builder.ToString());
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
