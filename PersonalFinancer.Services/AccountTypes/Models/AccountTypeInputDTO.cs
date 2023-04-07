namespace PersonalFinancer.Services.AccountTypes.Models
{
	public class AccountTypeInputDTO
    {
        public string Name { get; init; } = null!;

        public string OwnerId { get; set; } = null!;
    }
}
