namespace PersonalFinancer.Services.Shared.Models
{
	public class PaginationModel
	{
		public int ElementsPerPage { get; set; } = 10;

		public int Page { get; set; } = 1;

		public int TotalElements { get; set; }

		public int FirstElement 
			=> ElementsPerPage * (Page - 1) + 1;

		public int LastElement
		{
			get
			{
				int result = ElementsPerPage * Page;

				if (result > TotalElements)
				{
					result = TotalElements;
				}

				return result;
			}
		}

		public int Pages
		{
			get
			{
				int result = TotalElements / ElementsPerPage;

				if (TotalElements % ElementsPerPage != 0)
				{
					result++;
				}

				return result;
			}
		}
	}
}
