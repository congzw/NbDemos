using System.Web.Mvc;

namespace UnifyDemo.Controllers
{
    public class PagesController : Controller
    {
        //simple render a view
        public ActionResult Show(string category, string view)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return View(view);
            }
            return View(category + "/" + view);
        }
    }
}