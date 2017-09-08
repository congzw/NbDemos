using System.Web.Mvc;
using System.Web.Routing;

namespace UploadDemo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{view}",
                defaults: new { controller = "Demo", action = "Index", view = UrlParameter.Optional }
            );
        }
    }
}
