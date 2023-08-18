using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class SignInViewModel
{
	[EmailAddress(ErrorMessage = "Email formati yanlistir")]
	[Required(ErrorMessage = "Email alani bos biraxilamaz.")]
	[Display(Name = "Email: ")]
	public string Email { get; set; }

	[Required(ErrorMessage = "Parol alani bos biraxilamaz.")]
	[Display(Name = "Parol: ")]
	public string Password { get; set; }

	[Display(Name = "Beni xatirla ")]
	public bool RememberMe { get; set; }

	public SignInViewModel() { }

	public SignInViewModel(string email, string password)
	{
		Email = email;
		Password = password;
	}
}