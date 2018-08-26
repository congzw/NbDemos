using System.Web.Mvc;
using System.Web.Routing;
using SimpleMultiTenancy.Infrastructure;

namespace SimpleMultiTenancy
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //SetInitializers true means delete and create database!
            //ContextHelper.SetInitializers(true, ContextHelper.DefaultConn);
            ContextHelper.SetInitializers(false, ContextHelper.DefaultConn);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
