namespace PersonalFinancer.Services.Account.Models
{
    public class TransactionExtendedViewModel : TransactionShortViewModel
    {
        public string CategoryName { get; init; } = null!;

        public string Refference { get; init; } = null!;
    }
}
