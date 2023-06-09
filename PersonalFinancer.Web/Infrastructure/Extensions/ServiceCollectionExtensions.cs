namespace PersonalFinancer.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Caching.Memory;
    using PersonalFinancer.Data;
    using PersonalFinancer.Data.Models;
    using PersonalFinancer.Data.Models.Contracts;
    using PersonalFinancer.Data.Repositories;
    using PersonalFinancer.Services.Accounts;
    using PersonalFinancer.Services.ApiService;
    using PersonalFinancer.Services.Messages;
    using PersonalFinancer.Services.User;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            _ = services.AddScoped<IAccountsService, AccountsService>();
            _ = services.AddScoped<IApiService<AccountType>, ApiService<AccountType>>();
            _ = services.AddScoped<IApiService<Category>, ApiService<Category>>();
            _ = services.AddScoped<IApiService<Currency>, ApiService<Currency>>();
            _ = services.AddScoped<IEfRepository<Account>, EfRepository<Account>>();
            _ = services.AddScoped<IEfRepository<AccountType>, EfRepository<AccountType>>();
            _ = services.AddScoped<IEfRepository<ApplicationUser>, EfRepository<ApplicationUser>>();
            _ = services.AddScoped<IEfRepository<Category>, EfRepository<Category>>();
            _ = services.AddScoped<IEfRepository<Currency>, EfRepository<Currency>>();
            _ = services.AddScoped<IEfRepository<Transaction>, EfRepository<Transaction>>();
            _ = services.AddSingleton<IMemoryCache, MemoryCache>();
            _ = services.AddSingleton<IMongoDbContext, MessagesDbContext>();
            _ = services.AddSingleton<IMongoRepository<Message>, MongoRepository<Message>>();
            _ = services.AddSingleton<IMessagesService, MessagesService>();
            _ = services.AddScoped<IUsersService, UsersService>();
            _ = services.AddTransient<IEmailSender, EmailSender.EmailSender>();

            return services;
        }
    }
}
