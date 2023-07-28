namespace PersonalFinancer.Web.Extensions
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.UI.Services;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using Microsoft.Extensions.DependencyInjection;
	using PersonalFinancer.Data;
	using PersonalFinancer.Data.Configurations;
	using PersonalFinancer.Data.Models;
	using PersonalFinancer.Data.Repositories;
	using PersonalFinancer.Services.Accounts;
	using PersonalFinancer.Services.Api;
	using PersonalFinancer.Services.EmailSender;
	using PersonalFinancer.Services.Messages;
	using PersonalFinancer.Services.Users;
	using PersonalFinancer.Web.CustomFilters;
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
			var settings = builder.Configuration
				.GetSection("DefaultIdentityConfigurationSettings")
				.Get<DefaultIdentityConfigurationSettings>();

			builder.Services
				.AddDefaultIdentity<ApplicationUser>(options =>
				{
					options.SignIn.RequireConfirmedAccount = settings.RequireConfirmedAccount;
					options.User.RequireUniqueEmail = settings.RequireUniqueEmail;
					options.Password.RequireUppercase = settings.RequireUppercase;
					options.Password.RequireNonAlphanumeric = settings.RequireNonAlphanumeric;
					options.Password.RequiredLength = UserPasswordMinLength;
				})
				.AddRoles<IdentityRole<Guid>>()
				.AddEntityFrameworkStores<PersonalFinancerDbContext>();

			return builder;
		}

		public static WebApplicationBuilder ConfigureControllersWithViews(this WebApplicationBuilder builder)
		{
			builder.Services
				.AddControllersWithViews(options =>
				{
					options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
					options.Filters.Add(new HtmlSanitizeAsyncActionFilter());
				})
				.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

			return builder;
		}

		public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
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

				.AddTransient<IEmailSender, EmailSender>();

			return builder;
		}
	}
}
