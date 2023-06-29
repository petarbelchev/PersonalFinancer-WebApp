namespace PersonalFinancer.Web.Extensions
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.UI.Services;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using PersonalFinancer.Data;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.Cache;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.User;
	using PersonalFinancer.Web.EmailSender;
	using static PersonalFinancer.Common.Constants.UserConstants;

	public static class WebApplicationBuilderExtensions
	{
		public static WebApplicationBuilder ConfigurePersonalFinancerDbContext(this WebApplicationBuilder builder)
		{
			string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			builder.Services.AddDbContext<PersonalFinancerDbContext>(options => options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			return builder;
		}

		public static WebApplicationBuilder ConfigureDefaultIdentity(this WebApplicationBuilder builder)
		{
			builder.Services
				.AddDefaultIdentity<ApplicationUser>(options =>
				{
					options.SignIn.RequireConfirmedAccount = true;
					options.User.RequireUniqueEmail = true;
					options.Password.RequireUppercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequiredLength = UserPasswordMinLength;
				})
				.AddRoles<IdentityRole<Guid>>()
				.AddEntityFrameworkStores<PersonalFinancerDbContext>();

			return builder;
		}

		public static WebApplicationBuilder ConfigureControllersWithViews(this WebApplicationBuilder builder)
		{
			builder.Services
				.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
				.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

			return builder;
		}

		public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
		{
			builder.Services
				.AddScoped<IAccountsUpdateService, AccountsUpdateService>()
				.AddScoped<IAccountsInfoService, AccountsInfoService>()
				.AddScoped<IUsersService, UsersService>()
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
				.AddScoped<ICacheService<Category>, MemoryCacheService<Category>>()
				.AddScoped<ICacheService<Currency>, MemoryCacheService<Currency>>()
				.AddScoped<ICacheService<AccountType>, MemoryCacheService<AccountType>>()
				.AddScoped<ICacheService<Account>, MemoryCacheService<Account>>()

				.AddTransient<IEmailSender, EmailSender>();

			return builder;
		}
	}
}
