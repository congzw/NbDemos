using System.Web.Mvc;

namespace WidgetDemo.Controllers
{
    public class HomeController : NbControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
