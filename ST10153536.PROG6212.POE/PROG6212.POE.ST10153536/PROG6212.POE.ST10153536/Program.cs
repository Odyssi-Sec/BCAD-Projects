using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PROG6212.POE.ST10153536;
using PROG6212.POE.ST10153536.Interfaces;
using PROG6212.POE.ST10153536.Models;
using PROG6212.POE.ST10153536.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register IUserService and its implementation
builder.Services.AddTransient<IUserService, UserService>();

// Configure the application services and access configuration settings
var configuration = builder.Configuration;
builder.Services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));

// Configure authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Accounts/Login";
});


// Add the ApplicationDbContext service
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "moduleRoute",
    pattern: "Module/ModuleShow/{userId}",
    defaults: new { controller = "Module", action = "ModuleShow" });

app.MapControllerRoute(
    name: "graphRoute",
    pattern: "Graph/LineGraph/{userId}",
    defaults: new { controller = "Graph", action = "LineGraph" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/Accounts/Index", () => Results.Redirect("/Accounts/Login"));

app.Run();