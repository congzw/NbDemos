using System.Web.Mvc;

namespace UploadDemo.Controllers
{
    public class DemoController : Controller
    {
        public ActionResult Index(string view)
        {
            return View(view);
        }
    }
}