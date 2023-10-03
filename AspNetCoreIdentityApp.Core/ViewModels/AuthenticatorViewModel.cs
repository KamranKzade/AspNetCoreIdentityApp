using AspNetCoreIdentityApp.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Core.ViewModels;

public class AuthenticatorViewModel
{
	public string SharedKey { get; set; }
	public string AuthenticatorUri { get; set; }

	[Display(Name = "Doğrulama kodunuz")]
	[Required(ErrorMessage = "Doğrulama kodu gərəklidir")]
	public string VerificationCode { get; set; }

	public TwoFactor TwoFactorType { get; set; }
}
