global using Givehub.Models;
using Givehub.Helpers;
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

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.MapDefaultControllerRoute();
app.Run();
