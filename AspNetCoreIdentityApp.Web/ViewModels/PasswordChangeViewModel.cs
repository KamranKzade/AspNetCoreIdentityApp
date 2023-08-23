using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class PasswordChangeViewModel
{

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Kullanıcı Parol alanı boş bıraxılamaz.")]
	[Display(Name = "Parol: ")]
	[MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter ola bilir")]
	public string PasswordOld { get; set; } = null!;


	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Kullanıcı Yeni Parol alanı boş bıraxılamaz.")]
	[Display(Name = "Yeni Parol: ")]
	[MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter ola bilir")]
	public string PasswordNew { get; set; } = null!;

	[DataType(DataType.Password)]
	[Compare(nameof(PasswordNew), ErrorMessage = "Sifre ayni deyildir.")]
	[Required(ErrorMessage = "Kullanıcı Yeni Parol Təkrar alanı boş bıraxılamaz.")]
	[Display(Name = "Yeni Parol Təkrar: ")]
	[MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter ola bilir")]
	public string PasswordConfirm { get; set; } = null!;

}
