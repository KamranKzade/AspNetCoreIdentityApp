using Microsoft.AspNetCore.Http;
using AspNetCoreIdentityApp.Core.Models;
using System.ComponentModel.DataAnnotations;


namespace AspNetCoreIdentityApp.Core.ViewModels;


public class UserEditViewModel
{
	[Required(ErrorMessage = "Kullanıcı Ad alanı boş bıraxılamaz.")]
	[Display(Name = "Kullanıcı Adı: ")]
	public string Username { get; set; } = null!;

	[EmailAddress(ErrorMessage = "Email formati yanlistir")]
	[Required(ErrorMessage = "Kullanıcı Email alanı boş bıraxılamaz.")]
	[Display(Name = "Email: ")]
	public string Email { get; set; } = null!;

	[Required(ErrorMessage = "Kullanıcı Telefon alanı boş bıraxılamaz.")]
	[Display(Name = "Telefon: ")]
	public string Phone { get; set; } = null!;

	[DataType(DataType.Date)]
	[Display(Name = "Doğum tarixi : ")]
	public DateTime? BirtDate { get; set; }

	[Display(Name = "Şəhər : ")]
	public string? City { get; set; }

	[Display(Name = "Profil resmi : ")]
	public IFormFile? Picture { get; set; }

	[Display(Name = "Cinsiyyət: ")]
	public Gender? Gender { get; set; }
}
