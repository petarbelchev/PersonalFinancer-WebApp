namespace PersonalFinancer.Data.Models.Contracts
{
	using System;

	public abstract class CacheableEntity
	{
		public abstract string Name { get; set; }

		public abstract Guid OwnerId { get; set; }

		public abstract bool IsDeleted { get; set; }
	}
}
