using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidation;

public class PasswordValidator : IPasswordValidator<AppUser>
{
	public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
	{
		var error = new List<IdentityError>();

		if (password!.ToLower().Contains(user.UserName!.ToLower()))
		{
			error.Add(new IdentityError() { Code = "PasswordContainUserName", Description = "Parolda username ola bilmez" });
		}

		if (password.ToLower().StartsWith("1234"))
		{
			error.Add(new() { Code = "PasswordNoContaint1234", Description = "Parol 1234 ile baslaya bilmez" });
		}

		if (error.Any())
		{
			return Task.FromResult(IdentityResult.Failed(error.ToArray()));
		}

		return Task.FromResult(IdentityResult.Success);
	}
}