namespace PersonalFinancer.Web.Models.Shared
{
    public class AccountCardViewModel : AccountDropdownViewModel
    {
        public decimal Balance { get; set; }

        public string CurrencyName { get; set; } = null!;
    }
}
