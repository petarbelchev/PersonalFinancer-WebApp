namespace PersonalFinancer.Data.Models.Contracts
{
    public abstract class BaseApiEntity
    {
        public abstract Guid Id { get; set; }

		public abstract string Name { get; set; }

		public abstract Guid OwnerId { get; set; }

		public abstract bool IsDeleted { get; set; }
	}
}
