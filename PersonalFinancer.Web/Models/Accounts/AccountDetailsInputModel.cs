using System.ComponentModel.DataAnnotations;

using PersonalFinancer.Web.Models.Shared;

namespace PersonalFinancer.Web.Models.Accounts
{
	public class AccountDetailsInputModel : DateFilterInputModel
	{
		[Required]
		public string Id { get; set; } = null!;
	}
}
