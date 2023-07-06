namespace PersonalFinancer.Data.Repositories
{
    using MongoDB.Driver;
    using PersonalFinancer.Data.Models.Contracts;
    using System.Linq.Expressions;

    public interface IMongoRepository<T> where T : BaseMongoDocument
    {
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<int> CountAsync();

		/// <summary>
		/// Gets a value indicating whether the result is acknowledged.
		/// </summary>
		Task<DeleteResult> DeleteOneAsync(string documentId);

		Task<IEnumerable<TProjected>> FindAsync<TProjected>(
           Expression<Func<T, TProjected>> projectionExpression,
           int page = 1);

        Task<IEnumerable<TProjected>> FindAsync<TProjected>(
           Expression<Func<T, bool>> filterExpression,
           Expression<Func<T, TProjected>> projectionExpression,
           int page = 1);

		/// <summary>
		/// Throws Invalid Operation Exception when the document is not found with the given filter.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
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
