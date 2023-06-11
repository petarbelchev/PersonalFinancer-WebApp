using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinancer.Data;
using PersonalFinancer.Data.Configurations;
using PersonalFinancer.Data.Models;
using PersonalFinancer.Data.Seeding;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.Infrastructure.EmailSender;
using PersonalFinancer.Web.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PersonalFinancerDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SendGrid"));

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<PersonalFinancerDbContext>();

builder.Services
    .AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddServices();

builder.Services.AddAutoMapper(typeof(IAccountsService).Assembly, typeof(HomeController).Assembly);

builder.Services.ConfigureApplicationCookie(options => options.Cookie.HttpOnly = true);

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider serviceProvider = scope.ServiceProvider;

    PersonalFinancerDbContext dbContext =
        serviceProvider.GetRequiredService<PersonalFinancerDbContext>();

    new PersonalFinancerDbContextSeeder()
        .SeedAsync(dbContext, serviceProvider)
        .GetAwaiter()
        .GetResult();
}

if (app.Environment.IsDevelopment())
{
    _ = app.UseMigrationsEndPoint();
}
else
{
    _ = app.UseExceptionHandler("/Home/Error");
    _ = app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");
    _ = app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "Admin",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    _ = endpoints.MapDefaultControllerRoute();
});

app.MapRazorPages();

app.Run();