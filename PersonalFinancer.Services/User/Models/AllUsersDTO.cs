namespace PersonalFinancer.Services.User.Models
{
	public class AllUsersDTO
	{
		public IEnumerable<UserDTO> Users { get; set; } = null!;

        public int Page { get; set; }

        public int AllUsersCount { get; set; }
    }
}
