using System;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace UnifyDemo.Helpers
{
    public static class UnifyPageHelper
    {
        public static bool HideBreadcrumbs(this WebPageRenderingBase page)
        {
            if (page.Page.HideBreadcrumbs == null)
            {
                return false;
            }
            return page.Page.HideBreadcrumbs;
        }

        public static dynamic TryGetValue(this WebPageRenderingBase page, object key)
        {
            if (!page.PageData.ContainsKey(key))
            {
                return null;
            }
            var value = page.PageData[key];
            return value;
        }
    }
}
