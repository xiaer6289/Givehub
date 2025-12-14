global using Givehub.Models;
using Givehub.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Stripe;
//using Givehub.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<Givehub.Helpers.DoneeHelper>();
builder.Services.AddScoped<Givehub.Helpers.Helper>(); 
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DB>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSqlServer<DB>($@"
    Data Source=(LocalDB)\MSSQLLocalDB;
    AttachDbFilename={builder.Environment.ContentRootPath}\DB.mdf;
");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/NoPermission";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.Run();
