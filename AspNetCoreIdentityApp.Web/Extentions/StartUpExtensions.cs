using AspNetCoreIdentityApp.Web.CustomValidation;
using AspNetCoreIdentityApp.Web.Localizations;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

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
			opt.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";

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
}