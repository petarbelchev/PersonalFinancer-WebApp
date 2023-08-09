using PersonalFinancer.Data.Configurations;
using PersonalFinancer.Services.Accounts;
using PersonalFinancer.Web.Controllers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.ConfigurePersonalFinancerDbContext();

builder.Services.Configure<MongoDbConfigurationSettings>(
    builder.Configuration.GetSection("MongoDbConfigurationSettings"));

builder.ConfigureSendGridEmailSender();

builder.ConfigureDefaultIdentity();

builder.ConfigureControllersWithViews();

builder.AddApplicationServices();

builder.Services.AddSignalR();

builder.Services.AddAutoMapper(
    typeof(IAccountsUpdateService).Assembly, 
    typeof(HomeController).Assembly);

builder.ConfigureApplicationCookies();

builder.ConfigureDistributedRedisCache();

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
