using System.Web.Mvc;

namespace UploadDemo.Controllers
{
    public class UploadController : Controller
    {
        public ActionResult Index(string view)
        {
            return View(view);
        }
    }
}