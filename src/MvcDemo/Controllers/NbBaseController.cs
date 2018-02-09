//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Mvc;

//namespace MvcDemo.Controllers
//{
//    public abstract class NbBaseController : Controller
//    {
//        protected ActionResult RedirectToAction<TController>(Expression<Action<TController>> action)
//            where TController : Controller
//        {
//            return ControllerExtensions.RedirectToAction(this, action);
//        }

//        [Obsolete("Do not use the standard Json helpers to return JSON data to the client.  Use either JsonSuccess or JsonError instead.")]
//        protected JsonResult Json<T>(T data)
//        {
//            throw new InvalidOperationException("Do not use the standard Json helpers to return JSON data to the client.  Use either JsonSuccess or JsonError instead.");
//        }

//        protected StandardJsonResult JsonValidationError()
//        {
//            var result = new StandardJsonResult();

//            foreach (var validationError in System.Web.Mvc.ModelState.Values.SelectMany(v => v.Errors))
//            {
//                result.AddError(validationError.ErrorMessage);
//            }
//            return result;
//        }

//        protected StandardJsonResult JsonError(string errorMessage)
//        {
//            var result = new StandardJsonResult();

//            result.AddError(errorMessage);

//            return result;
//        }

//        protected StandardJsonResult<T> JsonSuccess<T>(T data)
//        {
//            return new StandardJsonResult<T> { Data = data };
//        }
//    }
//}
