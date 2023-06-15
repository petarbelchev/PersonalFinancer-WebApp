namespace PersonalFinancer.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Caching.Memory;
    using PersonalFinancer.Data;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.ApiService;
    using PersonalFinancer.Services.Messages;
    using PersonalFinancer.Services.User;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountsService, AccountsService>();
            services.AddScoped<IApiService<AccountType>, ApiService<AccountType>>();
            services.AddScoped<IApiService<Category>, ApiService<Category>>();
            services.AddScoped<IApiService<Currency>, ApiService<Currency>>();
            services.AddScoped<IEfRepository<Account>, EfRepository<Account>>();
            services.AddScoped<IEfRepository<AccountType>, EfRepository<AccountType>>();
            services.AddScoped<IEfRepository<ApplicationUser>, EfRepository<ApplicationUser>>();
            services.AddScoped<IEfRepository<Category>, EfRepository<Category>>();
            services.AddScoped<IEfRepository<Currency>, EfRepository<Currency>>();
            services.AddScoped<IEfRepository<Transaction>, EfRepository<Transaction>>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<IMongoDbContext, MessagesDbContext>();
            services.AddSingleton<IMongoRepository<Message>, MongoRepository<Message>>();
            services.AddSingleton<IMessagesService, MessagesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddTransient<IEmailSender, EmailSender.EmailSender>();

            return services;
        }
    }
}
