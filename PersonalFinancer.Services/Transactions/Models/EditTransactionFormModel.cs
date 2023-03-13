namespace PersonalFinancer.Services.Transactions.Models
{
    public class EditTransactionFormModel : CreateTransactionFormModel
    {
        public Guid Id { get; set; }

        public string AccountOwnerId { get; set; } = null!;

        public string? ReturnUrl { get; set; }
    }
}
