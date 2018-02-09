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
            return RedirectToAction("Index").WithError("Hello World!");
        }
	}
}