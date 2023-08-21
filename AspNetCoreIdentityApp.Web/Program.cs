using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

// Identittyni sisteme elave edirik
builder.Services.AddIdentityWithExtention();

// Cookie-ni appin Configuration-a tanitmaq
builder.Services.ConfigureApplicationCookie(opt =>
{
	var cookieBuilder = new CookieBuilder();
	cookieBuilder.Name = "UdemyCookie";
	
	// Login Path-in veririk
	opt.LoginPath = new PathString("/Home/SignIn");

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

app.UseAuthorization();

// Area-lar ile islemek ucun
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
