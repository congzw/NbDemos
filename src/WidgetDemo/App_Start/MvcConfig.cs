using System.Web.Mvc;
using System.Web.Routing;

namespace WidgetDemo
{
    public class MvcConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var defaultProjectPrefix = "WidgetDemo";
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Empty_Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new[] { string.Format("{0}.Controllers", defaultProjectPrefix) }
            );
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            ////异常处理
            //filters.Add(new NbExceptionHandleAttribute());

            ////事务控制
            //filters.Add(new MvcTransactionFilter());

            ////用户追踪
            //filters.Add(new UserTraceFilterAttribute());
        }
    }
}
