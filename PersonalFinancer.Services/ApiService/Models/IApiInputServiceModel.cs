namespace PersonalFinancer.Services.ApiService.Models
{
	public interface IApiInputServiceModel
    {
        public string Name { get; init; }

        public string OwnerId { get; set; }
    }
}
