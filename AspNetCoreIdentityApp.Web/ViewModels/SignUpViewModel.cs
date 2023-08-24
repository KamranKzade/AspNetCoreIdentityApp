using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;


public class SignUpViewModel
{
    [Required(ErrorMessage = "Kullanıcı Ad alanı boş bıraxılamaz.")]
    [Display(Name = "Kullanıcı Adi: ")]
    public string Username { get; set; } = null!;

	[EmailAddress(ErrorMessage ="Email formati yanlistir")]
	[Required(ErrorMessage = "Kullanıcı Email alanı boş bıraxılamaz.")]
	[Display(Name = "Email: ")]
    public string Email { get; set; } = null!;

	[Required(ErrorMessage = "Kullanıcı Telefon alanı boş bıraxılamaz.")]
	[Display(Name = "Telefon: ")]
    public string Phone { get; set; } = null!;

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Kullanıcı Parol alanı boş bıraxılamaz.")]
	[Display(Name = "Parol: ")]
	[MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter ola bilir")]
	public string Password { get; set; } = null!;

	[DataType(DataType.Password)]
	[Compare(nameof(Password),ErrorMessage ="Sifre ayni deyildir.")]
	[Required(ErrorMessage = "Kullanıcı Parol Təkrar alanı boş bıraxılamaz.")]
	[Display(Name = "Parol Təkrar: ")]
	[MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter ola bilir")]
	public string PasswordConfirm { get; set; } = null!;


	public SignUpViewModel() { }


    public SignUpViewModel(string username, string email, string phone, string password)
    {
        Username = username;
        Email = email;
        Phone = phone;
        Password = password;
    }

}