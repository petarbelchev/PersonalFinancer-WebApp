using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Services.Transactions.Models
{
    public class AllTransactionsServiceModel
    {
        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public IEnumerable<TransactionExtendedViewModel> Transactions { get; set; }
            = new List<TransactionExtendedViewModel>();
    }
}
