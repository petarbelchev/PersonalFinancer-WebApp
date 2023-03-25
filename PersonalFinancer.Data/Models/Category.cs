using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static PersonalFinancer.Data.Constants.TransactionConstants;

namespace PersonalFinancer.Data.Models
{
	public class Category
	{
		[Key]
		public string Id { get; set; } = null!;

		[MaxLength(CategoryNameMaxLength, 
			ErrorMessage = "Category's name max length must be {1} characters long.")]
		public string Name { get; set; } = null!;

		[ForeignKey(nameof(Owner))]
		public string OwnerId { get; set; } = null!;
		public ApplicationUser Owner { get; set; } = null!;

		public bool IsDeleted { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
