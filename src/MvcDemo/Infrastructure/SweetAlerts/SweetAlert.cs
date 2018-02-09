namespace MvcDemo.Infrastructure.SweetAlerts
{
	public class SweetAlert
	{
        public string AlertType { get; set; }
		public string AlertTitle { get; set; }

		public SweetAlert(string alertType, string alertTitle)
		{
			AlertType = alertType;
            AlertTitle = alertTitle;
		}
	}
}