namespace PersonalFinancer.Services.Transactions.Models
{
    public class TransactionExtendedViewModel : TransactionShortViewModel
    {
        public string CategoryName { get; init; } = null!;

        public string Refference { get; init; } = null!;
    }
}
