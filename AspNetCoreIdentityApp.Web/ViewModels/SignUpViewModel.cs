using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;


public class SignUpViewModel
{
    [Display(Name ="Kullanici Adi: ")]
    public string Username { get; set; }
    [Display(Name = "Email: ")]
    public string Email { get; set; }
    [Display(Name = "Telefon: ")]
    public string Phone { get; set; }
    [Display(Name = "Parol: ")]
    public string Password { get; set; }

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