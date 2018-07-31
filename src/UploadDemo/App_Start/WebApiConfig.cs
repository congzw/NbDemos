using System.Web.Http;

namespace UploadDemo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            
            config.Routes.MapHttpRoute(
                name: "RpcApi",
                routeTemplate: "Api/{controller}/{action}",
                defaults: new { }
            );
        }
    }
}
