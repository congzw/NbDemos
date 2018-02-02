using System.Web.Mvc;

namespace WidgetDemo.Controllers
{
    public class TemplateController : NbControllerBase
    {
        //Template/Feature/View.tmpl.cshtml
        public ActionResult Render(string feature, string view)
        {
            if (string.IsNullOrWhiteSpace(view))
            {
                return Content(string.Empty);
            }
            return View(view);
        }
    }
}