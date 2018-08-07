using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Microsoft.Web.Mvc;

namespace MvcDemo.Controllers
{
    public class CrudDataController : Controller
    {
        public CrudDataController()
        {
        }
        
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult New()
        //{
        //    var form = new NewIssueForm();
        //    return View(form);
        //}

        //[HttpPost]
        //public ActionResult New(NewIssueForm form)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(form);
        //    }

        //    //var assignedToUser = _context.Users.Single(u => u.Id == form.AssignedToUserID);

        //    //_context.Issues.Add(new Issue(_currentUser.User, assignedToUser, form.IssueType, form.Subject, form.Body));

        //    //_context.SaveChanges();

        //    return RedirectToAction<CrudDataController>(c => c.Index()).WithSuccess("...");
        //}

        //public ActionResult View(int id)
        //{
        //    var model = new object();
        //    //var model = _context.Issues
        //    //    .Project().To<IssueDetailsViewModel>()
        //    //    .SingleOrDefault(i => i.IssueID == id);

        //    //if (model == null)
        //    //{
        //    //    return RedirectToAction<HomeController>(c => c.Index())
        //    //        .WithError("Unable to find the issue.  Maybe it was deleted?");
        //    //}

        //    return View(model);
        //}

        //[HttpPost, Log("Saving changes")]
        //public ActionResult Edit(EditIssueForm form)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return JsonValidationError();
        //    }

        //    var issue = _context.Issues.SingleOrDefault(i => i.IssueID == form.IssueID);

        //    if (issue == null)
        //    {
        //        return JsonError("Cannot find the issue specified.");
        //    }

        //    var assignedToUser = _context.Users.Single(u => u.UserName == form.AssignedToUserName);

        //    issue.Subject = form.Subject;
        //    issue.AssignedTo = assignedToUser;
        //    issue.Body = form.Body;
        //    issue.IssueType = form.IssueType;

        //    return JsonSuccess(form);
        //}

        //[HttpPost, ValidateAntiForgeryToken, Log("Deleted issue {id}")]
        //public ActionResult Delete(int id)
        //{
        //    var issue = _context.Issues.Find(id);

        //    if (issue == null)
        //    {
        //        return RedirectToAction<HomeController>(c => c.Index())
        //            .WithError("Unable to find the issue.  Maybe it was deleted?");
        //    }

        //    _context.Issues.Remove(issue);

        //    _context.SaveChanges();

        //    return RedirectToAction<HomeController>(c => c.Index())
        //        .WithSuccess("Issue deleted!");
        //}
        
        #region helper

        protected ActionResult RedirectToAction<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            return ControllerExtensions.RedirectToAction(this, action);
        }

        #endregion
    }
}
