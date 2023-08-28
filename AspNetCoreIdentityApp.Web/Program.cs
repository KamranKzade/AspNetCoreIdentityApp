using AspNetCoreIdentityApp.Web.ClaimProviders;
using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.OptionsModels;
using AspNetCoreIdentityApp.Web.PermissionsRoot;
using AspNetCoreIdentityApp.Web.Requirements;
using AspNetCoreIdentityApp.Web.Seeds;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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

// wwwroot folderinde ola Userpicture folderine gede bilek deye, ozumuze referance noqtesi olaraq islediyimiz proyekti secdik
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));


// Burada biz framework-e basa saliriqki, hansisa 1 classin constructorunda
// IOptions<EmailSettings> gorsen, get datalari EmailSettingsden oxu
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Identittyni sisteme elave edirik
builder.Services.AddIdentityWithExtention();

// Request ve responce dongusu oldugu ucun scop veririk
builder.Services.AddScoped<IEmailService, EmailService>();

// Cookie elave etmek ucun --> Claim provider i sisteme tanidiriq
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();


// Userlere aid olan Claimler
builder.Services.AddAuthorization(opt =>
{
	// city-i Baku olanlari giris etmesi ucun yazdigimiz claim
	opt.AddPolicy("BakuPolicy", policy =>
	{
		policy.RequireClaim("city", "Baku");
	});

	// ExchangePolicy, hansi ki, uygun user ucun olacaq
	opt.AddPolicy("ExchangePolicy", policy =>
	{
		policy.AddRequirements(new ExchangeExpireRequirement());
	});

	// ViolencePolicy, hansi ki, siddet iceren sehifelere yasi 18den kicik olanlar gire bilmesin deye 1 mentiq qururuq 
	opt.AddPolicy("ViolencePolicy", policy =>
	{
		policy.AddRequirements(new ViolenceRequirement() { ThresholAge = 18 });
	});

	// ViolencePolicy, hansi ki, siddet iceren sehifelere yasi 18den kicik olanlar gire bilmesin deye 1 mentiq qururuq 
	opt.AddPolicy("OrderPermissionReadOrDelete", policy =>
	{
		policy.RequireClaim("Permission", Permissions.Order.Read);
		policy.RequireClaim("Permission", Permissions.Order.Delete);
		policy.RequireClaim("Permission", Permissions.Stock.Delete);
	});
});

// Cookie-ni appin Configuration-a tanitmaq
builder.Services.ConfigureApplicationCookie(opt =>
{
	var cookieBuilder = new CookieBuilder();
	cookieBuilder.Name = "UdemyCookie";

	// Login Path-in veririk --> hansi ki, girise icazesi olmayan userleri yonlendirdiyimiz sehifeler
	opt.LoginPath = new PathString("/Home/SignIn");
	opt.LogoutPath = new PathString("/Member/LogOut");
	opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
	opt.Cookie = cookieBuilder;
	// Cookie-nin muddeti
	opt.ExpireTimeSpan = TimeSpan.FromDays(30);
	// Kullanici expire time erzinde 1 defe giris etse, yeniden cookienin omru expire time qeder uzanir
	opt.SlidingExpiration = true;
});

var app = builder.Build();

// Permisionlari bura qeyd edirik
using (var scope = app.Services.CreateScope())
{
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();


	await PermissionSeed.Seed(roleManager);
}


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
