using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.Models;

public enum TwoFactor
{
	[Display(Name ="Heç biri")]
	None = 0,
	[Display(Name ="Telofon ilə kimlik doğrulama")]
	Phone = 1,
	[Display(Name ="Email ilə kimlik doğrulama")]
	Email = 2,
	[Display(Name ="Microsoft/Google Authenticator ilə kimlik doğrulama")]
	MicrosoftGoogle = 3
}
