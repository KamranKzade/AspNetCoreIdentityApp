using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class UserEditViewModel
{
	[Required(ErrorMessage = "Kullanıcı Ad alanı boş bıraxılamaz.")]
	[Display(Name = "Kullanıcı Adi: ")]
	public string Username { get; set; } = null!;

	[EmailAddress(ErrorMessage = "Email formati yanlistir")]
	[Required(ErrorMessage = "Kullanıcı Email alanı boş bıraxılamaz.")]
	[Display(Name = "Email: ")]
	public string Email { get; set; } = null!;

	[Required(ErrorMessage = "Kullanıcı Telefon alanı boş bıraxılamaz.")]
	[Display(Name = "Telefon: ")]
	public string Phone { get; set; } = null!;

	[Display(Name = "Doğum tarixi : ")]
	public DateTime? BirtDate { get; set; }

	[Display(Name = "Doğum Tarixi : ")]
	public string? City { get; set; }

	[Display(Name = "Profil resmi : ")]
	public IFormFile? Picture { get; set; }

	[Display(Name = "Cinsiyyət: ")]
	public byte? Gender { get; set; }
}
