namespace PersonalFinancer.Web.Models.Shared
{
	public class PaginationModel
	{
		public PaginationModel(
			string elementsName,
			int elementsPerPage,
			int totalElements,
			int page)
		{
			this.ElementsName = elementsName;
			this.ElementsPerPage = elementsPerPage;
			this.TotalElements = totalElements;
			this.Page = page;
		}

		public string ElementsName { get; private set; }

		public int ElementsPerPage { get; private set; }

		public int Page { get; private set; }

		public int TotalElements { get; private set; }

		public int FirstElement => this.TotalElements > 0
			? (this.ElementsPerPage * (this.Page - 1)) + 1
			: 0;

		public int LastElement
		{
			get
			{
				int result = this.ElementsPerPage * this.Page;

				if (result > this.TotalElements)
					result = this.TotalElements;

				return result;
			}
		}

		public int Pages
		{
			get
			{
				int result = this.TotalElements / this.ElementsPerPage;

				if (this.TotalElements % this.ElementsPerPage != 0)
					result++;

				if (result == 0)
					result = 1;

				return result;
			}
		}
	}
}
