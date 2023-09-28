using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using AspNetCoreIdentityApp.Web.Requirements;
using AspNetCoreIdentityApp.Service.Services;
using AspNetCoreIdentityApp.Web.Localizations;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Web.ClaimProviders;
using AspNetCoreIdentityApp.Web.CustomValidation;
using AspNetCoreIdentityApp.Core.PermissionsRoot;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Extentions;


// Program.cs file-nin pis gune qalmamasi ucun extention methodda yaziriq burani
public static class StartUpExtensions
{
	public static void AddIdentityWithExtention(this IServiceCollection services)
	{
		// Token-a omur vermek
		services.Configure<DataProtectionTokenProviderOptions>(options =>
		{
			options.TokenLifespan = TimeSpan.FromHours(2);
		});

		services.AddIdentity<AppUser, AppRole>(opt =>
		{
			// Identity-nin icerisinde gelen default validationlari oz isteyimize uygun duzeldirik
			opt.User.RequireUniqueEmail = true;
			opt.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_əöğşü-";

			opt.Password.RequiredLength = 6;
			opt.Password.RequireNonAlphanumeric = false;
			opt.Password.RequireLowercase = true;
			opt.Password.RequireUppercase = false;
			opt.Password.RequireDigit = false;

			// Lockout ile bagli melumatlar --> sehv giris eden zaman duzeldilmeli seyler
			opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
			opt.Lockout.MaxFailedAccessAttempts = 5;
		}).AddUserValidator<UserValidator>()
		.AddErrorDescriber<LocalizationIdentityErrorDescriber>()
		.AddPasswordValidator<PasswordValidator>()
		.AddDefaultTokenProviders() // Deafult token aliriq
		.AddEntityFrameworkStores<AppDbContext>();
	}

	public static void AddScopedWithExtention(this IServiceCollection services)
	{

		// Request ve responce dongusu oldugu ucun scop veririk
		services.AddScoped<IEmailService, EmailService>();

		// MemberService-i sisteme tanidiriq
		services.AddScoped<IMemberService, MemberService>();

		// Cookie elave etmek ucun --> Claim provider i sisteme tanidiriq
		services.AddScoped<IClaimsTransformation, UserClaimProvider>();
		services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
		services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();
			
	}

	public static void AddAuthorizationWithExtention(this IServiceCollection services)
	{
		// Userlere aid olan Claimler
		services.AddAuthorization(opt =>
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

			// Permissionlari veririk, Order ile bagli olan permissionlar ve basqa permissionlari
			opt.AddPolicy("Permissions.Order.Read", policy =>
			{
				policy.RequireClaim("Permission", Permissions.Order.Read);
			});

			// Permissionlari veririk, Order ile bagli olan permissionlar ve basqa permissionlari
			opt.AddPolicy("Permissions.Order.Delete", policy =>
			{
				policy.RequireClaim("Permission", Permissions.Order.Delete);
			});

			// Permissionlari veririk, Order ile bagli olan permissionlar ve basqa permissionlari
			opt.AddPolicy("Permissions.Stock.Delete", policy =>
			{
				policy.RequireClaim("Permission", Permissions.Stock.Delete);
			});
		});

	}

	public static void ConfigureApplicationCookieWithExtention(this IServiceCollection services)
	{
		// Cookie-ni appin Configuration-a tanitmaq
		services.ConfigureApplicationCookie(opt =>
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

	}

	public static void AddDbContextWithExtentions(this IServiceCollection services, IConfiguration configuration)
	{

		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("SqlCon"), options =>
			{
				options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
			});
		});

	}
}