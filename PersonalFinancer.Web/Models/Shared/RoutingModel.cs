namespace PersonalFinancer.Web.Models.Shared
{
	public class RoutingModel
	{
        public string Area { get; set; } = "";

        public string? Controller { get; set; }

        public string? Action { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
