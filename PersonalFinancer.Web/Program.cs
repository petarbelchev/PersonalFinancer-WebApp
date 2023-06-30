using PersonalFinancer.Data.Configurations;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.EmailSender;
using PersonalFinancer.Web.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.ConfigurePersonalFinancerDbContext();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<AuthEmailSenderOptions>(builder.Configuration.GetSection("SendGrid"));

builder.ConfigureDefaultIdentity();

builder.ConfigureControllersWithViews();

builder.AddServices();

builder.Services.AddAutoMapper(typeof(IAccountsUpdateService).Assembly, typeof(HomeController).Assembly);

builder.Services.ConfigureApplicationCookie(options => options.Cookie.HttpOnly = true);

WebApplication app = builder.Build();

app.SeedDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
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
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapDefaultControllerRoute();
});

app.MapRazorPages();

app.Run();