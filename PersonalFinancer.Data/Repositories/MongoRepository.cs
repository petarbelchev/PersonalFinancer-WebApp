namespace PersonalFinancer.Data.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
	using PersonalFinancer.Common.Constants;
	using PersonalFinancer.Data;
    using PersonalFinancer.Data.Models.Contracts;
    using System.Linq.Expressions;

    public class MongoRepository<T> : IMongoRepository<T> where T : BaseMongoDocument
    {
        private readonly IMongoCollection<T> collection;

        public MongoRepository(IMongoDbContext context)
            => this.collection = context.GetCollection<T>(typeof(T).Name);

		public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await this.collection.AsQueryable().AnyAsync(predicate);

        public async Task<int> CountAsync()
            => (int)await this.collection.CountDocumentsAsync(FilterDefinition<T>.Empty);

		public async Task<DeleteResult> DeleteOneAsync(string documentId)
            => await this.collection.DeleteOneAsync(x => x.Id == documentId);

		public async Task<IEnumerable<TProjected>> FindAsync<TProjected>(
			SortDefinition<T> sortDefinition,
			Expression<Func<T, TProjected>> projectionExpression,
			int page = 1)
		{
			return await this.collection
                .Find(Builders<T>.Filter.Empty)
                .Sort(sortDefinition)
                .Skip(PaginationConstants.MessagesPerPage * (page - 1))
                .Limit(PaginationConstants.MessagesPerPage)
                .Project(projectionExpression)
                .ToListAsync();
		}

		public async Task<IEnumerable<TProjected>> FindAsync<TProjected>(
			Expression<Func<T, bool>> filterExpression,
			SortDefinition<T> sortDefinition,
			Expression<Func<T, TProjected>> projectionExpression,
			int page = 1)
		{
			return await this.collection
				.Find(filterExpression)
				.Sort(sortDefinition)
				.Skip(PaginationConstants.MessagesPerPage * (page - 1))
				.Limit(PaginationConstants.MessagesPerPage)
				.Project(projectionExpression)
				.ToListAsync();
		}

		public async Task<TProjected> FindOneAsync<TProjected>(
			Expression<Func<T, bool>> filterExpression,
			Expression<Func<T, TProjected>> projectionExpression)
		{
			return await this.collection
				.Find(filterExpression)
				.Project(projectionExpression)
				.FirstAsync();
		}

		public async Task InsertOneAsync(T entity)
			=> await this.collection.InsertOneAsync(entity);

		public async Task<bool> IsUserDocumentAuthor(string documentId, string authorId)
		{
			return await this.collection
				.AsQueryable()
				.AnyAsync(x => x.Id == documentId && x.AuthorId == authorId);
		}

		public async Task<UpdateResult> UpdateOneAsync(
			Expression<Func<T, bool>> filterExpression,
			UpdateDefinition<T> update) 
			=> await this.collection.UpdateOneAsync(filterExpression, update);
	}
}
