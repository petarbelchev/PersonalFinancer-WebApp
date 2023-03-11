using System.ComponentModel.DataAnnotations;

namespace PersonalFinancer.Services.Shared.Models
{
	public class DateFilterModel
	{
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
		[DataType(DataType.DateTime)]
		[Display(Name = "Start Date")]
		public DateTime StartDate { get; set; }

		[Required(ErrorMessage = "End Date is required.")]
		[DataType(DataType.DateTime)]
		[Display(Name = "End Date")]
		public DateTime EndDate { get; set; }
	}
}
