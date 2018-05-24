using System.Web.Mvc;

namespace UnifyDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string view)
        {
            return View(view);
        }

        [HttpPost]
        public ActionResult PostData(FormCollection form)
        {
            return View("Blah");
        }
    }
}