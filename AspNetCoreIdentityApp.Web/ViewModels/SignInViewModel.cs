using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class SignInViewModel
{
	[EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
	[Required(ErrorMessage = "Email alanı boş bıraxılamaz.")]
	[Display(Name = "Email: ")]
	public string Email { get; set; } = null!;

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Parol alanı boş bıraxılamaz.")]
	[Display(Name = "Parol: ")]
	[MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter ola bilir")]
	public string Password { get; set; } = null!;

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