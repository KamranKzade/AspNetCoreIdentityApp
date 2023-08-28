using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidation;

public class PasswordValidator : IPasswordValidator<AppUser>
{
	public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
	{
		var errors = new List<IdentityError>();

		if (password!.ToLower().Contains(user.UserName!.ToLower()))
		{
			errors.Add(new IdentityError() { Code = "PasswordContainUserName", Description = "Parolda username ola bilmez" });
		}

		if (password.ToLower().StartsWith("1234"))
		{
			errors.Add(new() { Code = "PasswordContaint1234", Description = "Parol 1234 ile baslaya bilmez" });
		}

		if (errors.Any())
		{
			return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
		}

		return Task.FromResult(IdentityResult.Success);
	}
}