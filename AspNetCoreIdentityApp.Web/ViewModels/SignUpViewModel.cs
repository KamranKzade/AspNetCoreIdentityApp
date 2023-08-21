using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;


public class SignUpViewModel
{
    [Required(ErrorMessage = "Kullanıcı Ad alanı boş bıraxılamaz.")]
    [Display(Name = "Kullanıcı Adi: ")]
    public string Username { get; set; }

    [EmailAddress(ErrorMessage ="Email formati yanlistir")]
	[Required(ErrorMessage = "Kullanıcı Email alanı boş bıraxılamaz.")]
	[Display(Name = "Email: ")]
    public string Email { get; set; }

	[Required(ErrorMessage = "Kullanıcı Telefon alanı boş bıraxılamaz.")]
	[Display(Name = "Telefon: ")]
    public string Phone { get; set; }

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Kullanıcı Parol alanı boş bıraxılamaz.")]
	[Display(Name = "Parol: ")]
    public string Password { get; set; }

	[DataType(DataType.Password)]
	[Compare(nameof(Password),ErrorMessage ="Sifre ayni deyildir.")]
	[Required(ErrorMessage = "Kullanıcı Parol Təkrar alanı boş bıraxılamaz.")]
	[Display(Name = "Parol Təkrar: ")]
    public string PasswordConfirm { get; set; }


    public SignUpViewModel() { }


    public SignUpViewModel(string? username, string? email, string? phone, string? password)
    {
        Username = username;
        Email = email;
        Phone = phone;
        Password = password;
    }

}