using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace UploadDemo.Controllers
{
    public class UploadController : Controller
    {
        public ActionResult Index(string view)
        {
            return View(view);
        }
        
        [HttpPost]
        public ActionResult PostUpload(string returnView)
        {
            var fileNames = new List<string>();

            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        fileNames.Add(string.Format("{0}:[{1}k]", file.FileName, file.ContentLength / 1024));
                        ////todo save
                        //var fileName = Path.GetFileName(file.FileName);
                        //var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        //file.SaveAs(path);
                    }
                }
            }

            TempData["TempMessage"] = string.Join(",", fileNames);

            if (!string.IsNullOrWhiteSpace(returnView))
            {
                return RedirectToAction("Index", new {view = returnView});
            }
            return RedirectToAction("Index");
        }

        #region temp
        
        //[HttpPost]
        //public ActionResult Index(HttpPostedFileBase file)
        //{
        //    if (file != null && file.ContentLength > 0)
        //        try
        //        {
        //            string path = Path.Combine(Server.MapPath("~/Images"),
        //               Path.GetFileName(file.FileName));

        //            file.SaveAs(path);
        //            ViewBag.Message = "Your message for success";
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Message = "ERROR:" + ex.Message.ToString();
        //        }
        //    else
        //    {
        //        ViewBag.Message = "Please select file";
        //    }
        //    return View();
        //}

        #endregion
    }
}