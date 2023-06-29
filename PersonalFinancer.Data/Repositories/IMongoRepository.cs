namespace PersonalFinancer.Data.Repositories
{
    using MongoDB.Driver;
    using PersonalFinancer.Data.Models.Contracts;
    using System.Linq.Expressions;

    public interface IMongoRepository<T> where T : BaseMongoDocument
    {
        Task InsertOneAsync(T entity);

        Task<IEnumerable<TProjected>> FindAsync<TProjected>(
           Expression<Func<T, TProjected>> projectionExpression);

        Task<IEnumerable<TProjected>> FindAsync<TProjected>(
           Expression<Func<T, bool>> filterExpression,
           Expression<Func<T, TProjected>> projectionExpression);

		/// <summary>
		/// Throws Invalid Operation Exception when the document is not found with the given filter.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		Task<TProjected> FindOneAsync<TProjected>(
           Expression<Func<T, bool>> filterExpression,
           Expression<Func<T, TProjected>> projectionExpression);

        /// <summary>
        /// Gets a value indicating whether the result is acknowledged.
        /// </summary>
        Task<DeleteResult> DeleteOneAsync(string documentId);

        Task<UpdateResult> UpdateOneAsync(
           Expression<Func<T, bool>> filterExpression,
           UpdateDefinition<T> update);

        Task<bool> IsUserDocumentAuthor(string documentId, string authorId);
    }
}
