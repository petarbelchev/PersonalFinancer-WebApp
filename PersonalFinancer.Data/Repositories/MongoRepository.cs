using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PersonalFinancer.Data.Contracts;
using System.Linq.Expressions;

namespace PersonalFinancer.Data.Repositories
{
	public class MongoRepository<T> : IMongoRepository<T> where T : MongoDocument
	{
		private readonly IMongoCollection<T> collection;

		public MongoRepository(IMongoDbContext context)
			=> collection = context.GetCollection<T>(typeof(T).Name);

		public async Task InsertOneAsync(T entity)
			=> await collection.InsertOneAsync(entity);

		public async Task<IEnumerable<TProjected>> FindAsync<TProjected>(
			Expression<Func<T, TProjected>> projectionExpression)
			=> await collection
				.Find(Builders<T>.Filter.Empty)
				.Project(projectionExpression)
				.ToListAsync();

		public async Task<IEnumerable<TProjected>> FindAsync<TProjected>(
			Expression<Func<T, bool>> filterExpression,
			Expression<Func<T, TProjected>> projectionExpression)
			=> await collection
				.Find(filterExpression)
				.Project(projectionExpression)
				.ToListAsync();

		/// <summary>
		/// Throws InvalidOperationException when Document not found with the given filter.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TProjected> FindOneAsync<TProjected>(
			Expression<Func<T, bool>> filterExpression,
			Expression<Func<T, TProjected>> projectionExpression)
			=> await collection
				.Find(filterExpression)
				.Project(projectionExpression)
				.FirstAsync();

		/// <summary>
		/// Gets a value indicating whether the result is acknowledged.
		/// </summary>
		public async Task<DeleteResult> DeleteOneAsync(string documentId)
			=> await collection.DeleteOneAsync(x => x.Id == documentId);

		public async Task<UpdateResult> UpdateOneAsync(
			Expression<Func<T, bool>> filterExpression,
			UpdateDefinition<T> update)
			=> await collection.UpdateOneAsync(filterExpression, update);

		public async Task<bool> IsUserDocumentAuthor(string documentId, string authorId)
			=> await collection.AsQueryable().AnyAsync(x => x.Id == documentId && x.AuthorId == authorId);
	}
}
