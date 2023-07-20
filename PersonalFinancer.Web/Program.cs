using PersonalFinancer.Data.Configurations;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Web.Controllers;
using PersonalFinancer.Web.EmailSender;
using PersonalFinancer.Web.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.ConfigurePersonalFinancerDbContext();

builder.Services.Configure<MongoDbConfigurationSettings>(
    builder.Configuration.GetSection("MongoDbConfigurationSettings"));

builder.Services.Configure<AuthEmailSenderOptions>(
    builder.Configuration.GetSection("SendGrid"));

builder.ConfigureDefaultIdentity();

builder.ConfigureControllersWithViews();

builder.AddApplicationServices();

builder.Services.AddSignalR();

builder.Services.AddAutoMapper(
    typeof(IAccountsUpdateService).Assembly, 
    typeof(HomeController).Assembly);

builder.Services.ConfigureApplicationCookie(options => options.Cookie.HttpOnly = true);

WebApplication app = builder.Build();

app.SeedDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/StatusCodePage", "?statusCode={0}");

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

app.UseSignalRHubs();

app.MapRazorPages();

app.Run();
