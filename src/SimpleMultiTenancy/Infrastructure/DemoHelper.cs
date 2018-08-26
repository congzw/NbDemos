using System;
using System.Web;


namespace SimpleMultiTenancy.Infrastructure
{
    public class DemoHelper
    {
        //use request params for tenant demo
        public static string TryGetTentantCode(HttpContext current)
        {
            if (current == null)
            {
                return null;
            }
            var tenant = current.Request.Params["tenant"];
            if (tenant == null)
            {
                return null;
            }
            return tenant.Trim();
        }

        //use sub domain for tenant demo
        public static string GetUri(HttpContextBase context)
        {
            if (context == null || context.Request == null || context.Request.Url == null)
            {
                return null;
            }
            var uri = context.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped).ToLowerInvariant();
            return uri;
        }
    }
}