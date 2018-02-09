using System.Web.Mvc;

namespace MvcDemo.Infrastructure.SweetAlerts
{
	public class SweetAlertResult : ActionResult
	{
		public ActionResult InnerResult { get; set; }
	    public SweetAlert SweetAlert { get; set; }

		public SweetAlertResult(ActionResult innerResult, SweetAlert sweetAlert)
		{
            InnerResult = innerResult;
            SweetAlert = sweetAlert;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			var alerts = context.Controller.TempData.GetAlerts();
			alerts.Add(SweetAlert);
			InnerResult.ExecuteResult(context);
		}
	}
}