namespace PersonalFinancer.Services.Transactions.Models
{
    public class EditTransactionFormModel : TransactionFormModel
    {
        public Guid Id { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
