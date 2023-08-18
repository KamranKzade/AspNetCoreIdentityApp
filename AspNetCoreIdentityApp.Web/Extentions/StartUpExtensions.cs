using AspNetCoreIdentityApp.Web.CustomValidation;
using AspNetCoreIdentityApp.Web.Localizations;
using AspNetCoreIdentityApp.Web.Models;

namespace AspNetCoreIdentityApp.Web.Extentions;


// Program.cs file-nin pis gune qalmamasi ucun extention methodda yaziriq burani
public static class StartUpExtensions
{
	public static void AddIdentityWithExtention(this IServiceCollection services)
	{
		services.AddIdentity<AppUser, AppRole>(opt =>
		{
			// Identity-nin icerisinde gelen default validationlari oz isteyimize uygun duzeldirik
			opt.User.RequireUniqueEmail = true;
			opt.User.AllowedUserNameCharacters = "qwertyuiopasdfghjklzxcvbnm1234567890_";

			opt.Password.RequiredLength = 6;
			opt.Password.RequireNonAlphanumeric = false;
			opt.Password.RequireLowercase = true;
			opt.Password.RequireUppercase = false;
			opt.Password.RequireDigit = false;


		}).AddUserValidator<UserValidator>()
		.AddErrorDescriber<LocalizationIdentityErrorDescriber>()
		.AddPasswordValidator<PasswordValidator>()
		.AddEntityFrameworkStores<AppDbContext>();
	}
}