using AspNetCoreIdentityApp.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels;

public class AuthenticatorViewModel
{
	public string SharedKey { get; set; }
	public string AuthenticatorUri { get; set; }

	[Display(Name = "Doğrulama kodunuz")]
	[Required(ErrorMessage = "Doğrulama kodu gərəklidir")]
	public string VerificationCode { get; set; }

	[Display(Name ="İki adımlı kimlik doğrulama")]
	public TwoFactor TwoFactorType { get; set; }
}
