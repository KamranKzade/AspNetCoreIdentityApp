using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class ResetPasswordViewModel
{
	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Kullanıcı Parol alanı boş bıraxılamaz.")]
	[Display(Name = "Yeni Parol: ")]
	public string Password { get; set; }

	[DataType(DataType.Password)]
	[Compare(nameof(Password), ErrorMessage = "Sifre ayni deyildir.")]
	[Required(ErrorMessage = "Kullanıcı Parol Təkrar alanı boş bıraxılamaz.")]
	[Display(Name = "Yeni Parol Təkrar: ")]
	public string PasswordConfirm { get; set; }
}
