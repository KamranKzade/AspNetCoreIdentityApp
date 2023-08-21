using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class SignInViewModel
{
	[EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
	[Required(ErrorMessage = "Email alanı boş bıraxılamaz.")]
	[Display(Name = "Email: ")]
	public string Email { get; set; }

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Parol alanı boş bıraxılamaz.")]
	[Display(Name = "Parol: ")]
	public string Password { get; set; }

	[DataType(DataType.Password)]
	[Display(Name = "Məni xatırla ")]
	public bool RememberMe { get; set; }

	public SignInViewModel() { }

	public SignInViewModel(string email, string password)
	{
		Email = email;
		Password = password;
	}
}