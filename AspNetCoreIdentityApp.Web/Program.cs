using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.OptionsModels;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

// Security Stamp-a interval vermek
builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
{
	opt.ValidationInterval = TimeSpan.FromMinutes(30);
});

// Burada biz framework-e basa saliriqki, hansisa 1 classin constructorunda
// IOptions<EmailSettings> gorsen, get datalari EmailSettingsden oxu
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Identittyni sisteme elave edirik
builder.Services.AddIdentityWithExtention();

// Request ve responce dongusu oldugu ucun scop veririk
builder.Services.AddScoped<IEmailService, EmailService>();

// Cookie-ni appin Configuration-a tanitmaq
builder.Services.ConfigureApplicationCookie(opt =>
{
	var cookieBuilder = new CookieBuilder();
	cookieBuilder.Name = "UdemyCookie";

	// Login Path-in veririk
	opt.LoginPath = new PathString("/Home/SignIn");
	opt.LogoutPath = new PathString("/Member/LogOut");
	opt.Cookie = cookieBuilder;
	// Cookie-nin muddeti
	opt.ExpireTimeSpan = TimeSpan.FromDays(30);
	// Kullanici expire time erzinde 1 defe giris etse, yeniden cookienin omru expire time qeder uzanir
	opt.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
// Area-lar ile islemek ucun
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
