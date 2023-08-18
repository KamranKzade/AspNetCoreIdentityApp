using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Localizations;

public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
{
	public override IdentityError DuplicateUserName(string userName)
	{
		return new IdentityError()
		{
			Code = "DuplicateUsername",
			Description = $"{userName} daha once basqa bir kullanici tarafindan alinmisdir"
		};
	}

	public override IdentityError DuplicateEmail(string email)
	{
		return new IdentityError()
		{
			Code = "DuplicateEmail",
			Description = $"{email} daha once basqa bir kullanici tarafindan alinmisdir"
		};
	}

	public override IdentityError PasswordTooShort(int length)
	{
		return new IdentityError()
		{
			Code = "PasswordTooShort",
			Description = $"Parol en az {length} karakterli olmalidir"
		};
	}
}