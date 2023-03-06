namespace PersonalFinancer.Services.AccountTypes.Models
{
    public class AccountTypeViewModel
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = null!;

        public string? UserId { get; set; }
    }
}
