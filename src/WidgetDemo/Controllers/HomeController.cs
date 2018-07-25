using System.Web.Mvc;

namespace WidgetDemo.Controllers
{
    public class HomeController : NbControllerBase
    {
        public ActionResult Index(string view)
        {
            if (!string.IsNullOrWhiteSpace(view))
            {
                return View(view);
            }
            return View();
        }
    }
}
