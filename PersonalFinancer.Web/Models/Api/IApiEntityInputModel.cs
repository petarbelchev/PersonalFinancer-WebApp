namespace PersonalFinancer.Web.Models.Api
{
	public interface IApiEntityInputModel
	{
        public string Name { get; set; }

        public Guid? OwnerId { get; set; }
    }
}
