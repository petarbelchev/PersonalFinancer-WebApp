namespace PersonalFinancer.Web.Models.Api
{
	using System.ComponentModel.DataAnnotations;

	public class SearchFilterInputModel
	{
		[Required]
		[Range(1, int.MaxValue)]
		public int Page { get; set; }

		public string? Search { get; set; }
	}
}
