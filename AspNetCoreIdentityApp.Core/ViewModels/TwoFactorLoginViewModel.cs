using AspNetCoreIdentityApp.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels;

public class TwoFactorLoginViewModel
{
	[Display(Name = "Doğrulama kodunuz")]
	[Required(ErrorMessage = "Doğrulama kodu boş olamaz")]
	[StringLength(8, ErrorMessage = "Doğrulama kodunuz ən çox 8 haneli ola bilər")]
	public string VerificationCode { get; set; }


	public bool isRememberMe { get; set; }

	public bool isRecoveryCode { get; set; }

	public TwoFactor TwoFactorType { get; set; }
}
