namespace PersonalFinancer.Web.Models.Api
{
	public interface IApiInputModel
	{
        public string Name { get; set; }

        public Guid? OwnerId { get; set; }
    }
}
