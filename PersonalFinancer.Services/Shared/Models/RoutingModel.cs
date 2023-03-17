namespace PersonalFinancer.Services.Shared.Models
{
	public class RoutingModel
	{
        public string Area { get; set; } = "";

        public string? Controller { get; set; }

        public string? Action { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
