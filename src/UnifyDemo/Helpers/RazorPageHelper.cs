using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace UnifyDemo.Helpers
{
    public static class RazorPageHelper
    {
        /// <summary>
        /// 如果@section未定义，则渲染部分页
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="page"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IHtmlString RenderSectionOrPartial<TModel>(this WebViewPage<TModel> page, string name)
        {
            var renderSection = page.RenderSection(name, false);
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
            page.DefineSection(name, () =>
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
            page.DefineSection(name, () =>
            {

            });
            return null;
        }
    }
}
