using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;


public class SignUpViewModel
{
    [Required(ErrorMessage = "Kullanici Ad alani bos biraxilamaz")]
    [Display(Name ="Kullanici Adi: ")]
    public string Username { get; set; }

    [EmailAddress(ErrorMessage ="Email formati yanlistir")]
	[Required(ErrorMessage = "Kullanici Email alani bos biraxilamaz.")]
	[Display(Name = "Email: ")]
    public string Email { get; set; }

	[Required(ErrorMessage = "Kullanici Telefon alani bos biraxilamaz.")]
	[Display(Name = "Telefon: ")]
    public string Phone { get; set; }


	[Required(ErrorMessage = "Kullanici Parol alani bos biraxilamaz.")]
	[Display(Name = "Parol: ")]
    public string Password { get; set; }

    [Compare(nameof(Password),ErrorMessage ="Sifre ayni deyildir.")]
	[Required(ErrorMessage = "Kullanici Parol Tekrar alani bos biraxilamaz")]
	[Display(Name = "Parol Tekrar: ")]
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