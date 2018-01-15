using System.Web.Mvc;

namespace FormAuthDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Admin()
        {
            return View();
        }
    }
}
