namespace PersonalFinancer.Data.Repositories
{
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using Microsoft.EntityFrameworkCore;

    using System.Linq.Expressions;

    using Data.Contracts;

    public class MongoRepository<T> : IMongoRepository<T> where T : MongoDocument
	{
		private IMongoCollection<T> collection;

		public MongoRepository(IMongoDbContext context)
			=> collection = context.GetCollection<T>(typeof(T).Name);

		public async Task InsertOneAsync(T entity)
			=> await collection.InsertOneAsync(entity);

		public async Task<IEnumerable<TProjected>> FindAsync<TProjected>(
			Expression<Func<T, TProjected>> projectionExpression)
		{
			var result = await collection
				.Find(Builders<T>.Filter.Empty)
				.Project(projectionExpression)
				.ToListAsync();

			return result;
		}
		
		public async Task<IEnumerable<TProjected>> FindAsync<TProjected>(
			Expression<Func<T, bool>> filterExpression,
			Expression<Func<T, TProjected>> projectionExpression)
		{
			var result = await collection
				.Find(filterExpression)
				.Project(projectionExpression)
				.ToListAsync();

			return result;
		}

		/// <summary>
		/// Throws InvalidOperationException when Document not found with the given filter.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<TProjected> FindOneAsync<TProjected>(
			Expression<Func<T, bool>> filterExpression,
			Expression<Func<T, TProjected>> projectionExpression)
		{
			var result = await collection
				.Find(filterExpression)
				.Project(projectionExpression)
				.FirstAsync();

			return result;
		}

		public async Task<DeleteResult> DeleteOneAsync(string documentId)
		{
			var result = await collection.DeleteOneAsync(x => x.Id == documentId);
			return result;
		}

		public async Task<UpdateResult> UpdateOneAsync(
			Expression<Func<T, bool>> filterExpression,
			UpdateDefinition<T> update)
		{
			var result = await collection.UpdateOneAsync(filterExpression, update);
			return result;
		}

		public async Task<bool> IsUserDocumentAuthor(string documentId, string authorId)
		{
			bool result = await collection.AsQueryable().AnyAsync(x => x.Id == documentId && x.AuthorId == authorId);

			return result;
		}
	}
}
