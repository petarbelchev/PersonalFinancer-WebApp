namespace PersonalFinancer.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Caching.Memory;
    using PersonalFinancer.Data;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.ApiService;
	using PersonalFinancer.Services.MemoryCacheService;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Transactions;
	using PersonalFinancer.Services.User;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IAccountsService, AccountsService>()
                .AddScoped<IUsersService, UsersService>()
                .AddScoped<ITransactionsInfoService, TransactionsInfoService>()
                .AddSingleton<IMessagesService, MessagesService>()

                .AddScoped<IApiService<AccountType>, ApiService<AccountType>>()
                .AddScoped<IApiService<Category>, ApiService<Category>>()
                .AddScoped<IApiService<Currency>, ApiService<Currency>>()
            
                .AddScoped<IEfRepository<Account>, EfRepository<Account>>()
                .AddScoped<IEfRepository<AccountType>, EfRepository<AccountType>>()
                .AddScoped<IEfRepository<ApplicationUser>, EfRepository<ApplicationUser>>()
                .AddScoped<IEfRepository<Category>, EfRepository<Category>>()
                .AddScoped<IEfRepository<Currency>, EfRepository<Currency>>()
                .AddScoped<IEfRepository<Transaction>, EfRepository<Transaction>>()
            
                .AddSingleton<IMongoRepository<Message>, MongoRepository<Message>>()
            
                .AddSingleton<IMongoDbContext, MessagesDbContext>()
            
                .AddSingleton<IMemoryCache, MemoryCache>()
                .AddScoped<IMemoryCacheService<Category>, MemoryCacheService<Category>>()
                .AddScoped<IMemoryCacheService<Currency>, MemoryCacheService<Currency>>()
                .AddScoped<IMemoryCacheService<AccountType>, MemoryCacheService<AccountType>>()
                .AddScoped<IMemoryCacheService<Account>, MemoryCacheService<Account>>()

                .AddTransient<IEmailSender, EmailSender.EmailSender>();
			
            return services;
        }
    }
}
