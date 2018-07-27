using System.Web.Mvc;

namespace AceDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string view)
        {
            if (string.IsNullOrWhiteSpace(view))
            {
                return View();
            }
            return View(view);
        }
    }
}