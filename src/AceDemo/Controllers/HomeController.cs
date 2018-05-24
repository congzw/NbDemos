using System.Web.Mvc;

namespace AceDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string view)
        {
            return View(view);
        }
    }
}