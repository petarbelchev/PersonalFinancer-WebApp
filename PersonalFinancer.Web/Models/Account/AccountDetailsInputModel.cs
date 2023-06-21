namespace PersonalFinancer.Web.Models.Account
{
	using PersonalFinancer.Services.Shared.Models;
	using System.ComponentModel.DataAnnotations;

	public class AccountDetailsInputModel : DateFilterModel
	{
		[Required]
		public Guid? Id { get; set; }
	}
}
