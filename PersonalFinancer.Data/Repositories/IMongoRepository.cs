namespace PersonalFinancer.Data.Repositories
{
    using MongoDB.Driver;
    using PersonalFinancer.Data.Models.Contracts;
    using System.Linq.Expressions;

    public interface IMongoRepository<T> where T : BaseMongoDocument
    {
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<int> CountAsync();

		Task<DeleteResult> DeleteOneAsync(string documentId);

		Task<IEnumerable<TProjected>> FindAsync<TProjected>(
			SortDefinition<T> sortDefinition,
			Expression<Func<T, TProjected>> projectionExpression,
			int page = 1);

		Task<IEnumerable<TProjected>> FindAsync<TProjected>(
			Expression<Func<T, bool>> filterExpression,
			SortDefinition<T> sortDefinition,
			Expression<Func<T, TProjected>> projectionExpression,
			int page = 1);

		/// <exception cref="InvalidOperationException">When the document is not found with the given filter.</exception>
		Task<TProjected> FindOneAsync<TProjected>(
			Expression<Func<T, bool>> filterExpression,
			Expression<Func<T, TProjected>> projectionExpression);

		Task InsertOneAsync(T entity);

		Task<bool> IsUserDocumentAuthor(string documentId, string authorId);

        Task<UpdateResult> UpdateOneAsync(
			Expression<Func<T, bool>> filterExpression,
			UpdateDefinition<T> update);
    }
}
