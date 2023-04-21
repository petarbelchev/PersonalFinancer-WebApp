﻿using PersonalFinancer.Data.Models;

namespace PersonalFinancer.Services.Infrastructure
{
	public interface IEfRepository<T> where T : class
	{
		Task AddAsync(T entity);

		Task<int> SaveChangesAsync();

		void Remove(T entity);

		IQueryable<T> All();

		Task<T?> FindAsync(string id);
	}
}
