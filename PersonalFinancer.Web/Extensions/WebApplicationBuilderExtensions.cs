namespace Microsoft.Extensions.DependencyInjection
{
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.UI.Services;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
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
	using SendGrid.Extensions.DependencyInjection;
	using static PersonalFinancer.Common.Constants.UserConstants;

	public static class WebApplicationBuilderExtensions
	{
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

				.AddTransient<IEmailSender, SendGridEmailSender>();

			return builder;
		}

		public static WebApplicationBuilder ConfigureApplicationCookies(this WebApplicationBuilder builder)
		{
			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.Name = "PersonalFinancerCookie";
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
				options.Cookie.SameSite = SameSiteMode.Strict;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			});

			builder.Services.AddAntiforgery(options =>
			{
				options.Cookie.Name = "AntiforgeryCookie";
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
				options.Cookie.SameSite = SameSiteMode.Strict;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			});

			return builder;
		}

		public static WebApplicationBuilder ConfigureControllersWithViews(this WebApplicationBuilder builder)
		{
			builder.Services
				.AddControllersWithViews(options =>
				{
					options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
					options.Filters.Add(new HtmlSanitizeActionFilter());
				})
				.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

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

		public static WebApplicationBuilder ConfigurePersonalFinancerDbContext(this WebApplicationBuilder builder)
		{
			string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			builder.Services.AddDbContext<PersonalFinancerDbContext>(options => options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			return builder;
		}

		public static WebApplicationBuilder ConfigureSendGridEmailSender(this WebApplicationBuilder builder)
		{
			builder.Services.Configure<AuthEmailSenderOptions>(builder.Configuration.GetSection("SendGrid"));

			builder.Services.AddSendGrid(options => options.ApiKey = builder.Configuration.GetValue<string>("SendGrid:SendGridKey"));

			return builder;
		}
	}
}
