using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using MvcDemo.MultiTenancy;

namespace MvcDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MockHelper.SetupIoc();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
