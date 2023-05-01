using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Contracts;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Repositories;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.ApiService;
using PersonalFinancer.Services.Messages;
using PersonalFinancer.Services.User;

using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.Infrastructure;
using PersonalFinancer.Web.Infrastructure.EmailSender;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SqlDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<MongoDbSettings>(
	builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<AuthMessageSenderOptions>(
	builder.Configuration.GetSection("SendGrid"));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = true;
	options.User.RequireUniqueEmail = true;
	options.Password.RequireNonAlphanumeric = false;
})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<SqlDbContext>();

builder.Services.AddControllersWithViews(options =>
{
	options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
})
	.ConfigureApiBehaviorOptions(options =>
	{
		options.SuppressModelStateInvalidFilter = true;
	});

builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<IApiService<AccountType>, ApiService<AccountType>>();
builder.Services.AddScoped<IApiService<Category>, ApiService<Category>>();
builder.Services.AddScoped<IApiService<Currency>, ApiService<Currency>>();
builder.Services.AddScoped<IEfRepository<Account>, EfRepository<Account>>();
builder.Services.AddScoped<IEfRepository<AccountType>, EfRepository<AccountType>>();
builder.Services.AddScoped<IEfRepository<ApplicationUser>, EfRepository<ApplicationUser>>();
builder.Services.AddScoped<IEfRepository<Category>, EfRepository<Category>>();
builder.Services.AddScoped<IEfRepository<Currency>, EfRepository<Currency>>();
builder.Services.AddScoped<IEfRepository<Transaction>, EfRepository<Transaction>>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddSingleton<IMongoRepository<Message>, MongoRepository<Message>>();
builder.Services.AddSingleton<IMessagesService, MessagesService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddAutoMapper(
	typeof(IAccountsService).Assembly,
	typeof(HomeController).Assembly);

builder.Services.ConfigureApplicationCookie(options =>
{
	options.AccessDeniedPath = "/Home/AccessDenied";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
		name: "Admin",
		pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
	);

	endpoints.MapDefaultControllerRoute();
});

app.MapRazorPages();

app.SeedUserRoles();
app.SeedAccountsAndTransactions();

app.Run();