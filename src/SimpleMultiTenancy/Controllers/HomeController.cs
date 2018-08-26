using System.Web.Mvc;

namespace SimpleMultiTenancy.Controllers
{
    public class HomeController : Controller
    {
        //demo for each tenant infos(per database)!
        public ActionResult Index()
        {
            return View();
        }

        //demo for all tenants manage
        public ActionResult Tenants()
        {
            return View();
        }

    }
}
