namespace PersonalFinancer.Services.Accounts.Models
{
	public interface IHaveOwner
	{
		public Guid OwnerId { get; set; }
	}
}
