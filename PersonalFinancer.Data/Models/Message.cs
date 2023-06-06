﻿using PersonalFinancer.Data.Contracts;

namespace PersonalFinancer.Data.Models
{
	public class Message : MongoDocument
	{
		public string AuthorName { get; set; } = null!;

		public DateTime CreatedOn { get; set; }

		public string Subject { get; set; } = null!;

		public string Content { get; set; } = null!;

		public ICollection<Reply> Replies { get; set; }
			= new List<Reply>();
	}
}
