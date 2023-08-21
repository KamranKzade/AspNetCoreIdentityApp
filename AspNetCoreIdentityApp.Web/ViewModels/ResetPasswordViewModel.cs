using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class ResetPasswordViewModel
{
	[EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
	[Required(ErrorMessage = "Email alanı boş bıraxılamaz.")]
	[Display(Name = "Email: ")]
	public string Email { get; set; }
}
