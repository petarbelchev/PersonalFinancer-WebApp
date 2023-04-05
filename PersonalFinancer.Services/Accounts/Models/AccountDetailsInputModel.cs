using PersonalFinancer.Services.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Services.Accounts.Models
{
	public class AccountDetailsInputModel : DateFilterModel
	{
		[Required]
		public string Id { get; set; } = null!;
	}
}
