namespace PersonalFinancer.Web.Models.Shared
{
    public class PaginationModel
    {
        public string ElementsName { get; set; } = null!;

        public int ElementsPerPage { get; set; }

        public int Page { get; set; } = 1;

        public int TotalElements { get; set; }

        public int FirstElement => (this.ElementsPerPage * (this.Page - 1)) + 1;

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

                return result;
            }
        }
    }
}
