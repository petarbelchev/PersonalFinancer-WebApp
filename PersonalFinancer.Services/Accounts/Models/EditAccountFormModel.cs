namespace PersonalFinancer.Services.Accounts.Models
{
	public class EditAccountFormModel : CreateAccountFormModel
	{
        public Guid Id { get; set; }

        public string OwnerId { get; set; } = null!;

        public string? ReturnUrl { get; set; }
    }
}
