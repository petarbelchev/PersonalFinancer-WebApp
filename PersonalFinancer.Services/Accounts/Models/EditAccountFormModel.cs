namespace PersonalFinancer.Services.Accounts.Models
{
	public class EditAccountFormModel : AccountFormModel
	{
        public Guid Id { get; set; }

        public string OwnerId { get; set; } = null!;

        public string? ReturnUrl { get; set; }
    }
}
