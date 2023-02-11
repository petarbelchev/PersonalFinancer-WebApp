using Microsoft.EntityFrameworkCore;

using PersonalFinancer.Data;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Services.Account;
using PersonalFinancer.Services.Category;
using PersonalFinancer.Services.Currency;
using PersonalFinancer.Services.User;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PersonalFinancerDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	options.User.RequireUniqueEmail = true;
	options.Password.RequireNonAlphanumeric = false;
})
	.AddEntityFrameworkStores<PersonalFinancerDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/User/Login";
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();