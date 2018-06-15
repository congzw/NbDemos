using System.Web.Mvc;
using MvcDemo.Infrastructure.SweetAlerts;

namespace MvcDemo.Controllers
{
	public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Demo()
        {
            return View().WithInfo("Hello World From Server!");
        }

        public ActionResult Demo2()
        {
            return RedirectToAction("Index").WithError("Hello World From Server!");
        }
	}
}