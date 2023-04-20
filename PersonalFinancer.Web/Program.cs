using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;

using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Services.ApiService;
using PersonalFinancer.Services.Messages;
using PersonalFinancer.Services.User;

using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PersonalFinancerDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<MongoDbSettings>(
	builder.Configuration.GetSection("MessagesDatabase"));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	options.User.RequireUniqueEmail = true;
	options.Password.RequireNonAlphanumeric = false;
})
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<PersonalFinancerDbContext>();

builder.Services.AddControllersWithViews(options =>
{
	options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
})
	.ConfigureApiBehaviorOptions(options =>
	{
		options.SuppressModelStateInvalidFilter = true;
	});

builder.Services.AddScoped<IAccountsService, AccountsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<ApiService<AccountType>, ApiService<AccountType>>();
builder.Services.AddScoped<ApiService<Category>, ApiService<Category>>();
builder.Services.AddScoped<ApiService<Currency>, ApiService<Currency>>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddSingleton<MongoDbContext, MongoDbContext>();
builder.Services.AddSingleton<MessagesService>();

builder.Services.AddAutoMapper(
	typeof(IAccountsService).Assembly,
	typeof(HomeController).Assembly);

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/User/Login";
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
	app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");

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