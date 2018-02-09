using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcDemo.Infrastructure.SweetAlerts
{
	public static class SweetAlertExtensions
	{
		const string SweetAlerts = "_SweetAlerts";

		public static SweetAlert GetAlert(this TempDataDictionary tempData)
		{
			if (!tempData.ContainsKey(SweetAlerts))
			{
				tempData[SweetAlerts] = new List<SweetAlert>();
			}

			return ((List<SweetAlert>)tempData[SweetAlerts]).LastOrDefault();
		}

		public static List<SweetAlert> GetAlerts(this TempDataDictionary tempData)
		{
			if (!tempData.ContainsKey(SweetAlerts))
			{
				tempData[SweetAlerts] = new List<SweetAlert>();
			}

			return (List<SweetAlert>) tempData[SweetAlerts];
		}

		public static ActionResult WithSuccess(this ActionResult result, string message)
		{
			return new SweetAlertResult(result, new SweetAlert("success", message));
		}

		public static ActionResult WithInfo(this ActionResult result, string message)
		{
			return new SweetAlertResult(result, new SweetAlert("info", message));
		}

		public static ActionResult WithWarning(this ActionResult result, string message)
		{
			return new SweetAlertResult(result, new SweetAlert("warning", message));
		}

		public static ActionResult WithError(this ActionResult result, string message)
		{
			return new SweetAlertResult(result, new SweetAlert("error", message));
		}

		public static ActionResult WithQuestion(this ActionResult result, string message)
		{
			return new SweetAlertResult(result, new SweetAlert("question", message));
		}
	}
}