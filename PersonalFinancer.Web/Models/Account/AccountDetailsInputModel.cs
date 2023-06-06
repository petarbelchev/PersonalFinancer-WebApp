using PersonalFinancer.Web.Models.Shared;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Web.Models.Account
{
	public class AccountDetailsInputModel : DateFilterModel
	{
		[Required]
		public string Id { get; set; } = null!;
	}
}
