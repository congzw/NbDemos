using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace UnifyDemo.Helpers
{
    public static class RazorPageHelper
    {
        public static IHtmlString RenderSectionOrPartial<TModel>(this WebViewPage<TModel> page, string name)
        {
            var renderSection = page.RenderSection(name, false);
            if (renderSection != null)
            {
                return renderSection;
            }
            return page.Html.Partial(name);
        }
    }
}
