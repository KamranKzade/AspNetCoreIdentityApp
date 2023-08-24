﻿using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class ForgetPasswordViewModel
{
	[EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
	[Required(ErrorMessage = "Email alanı boş bıraxılamaz.")]
	[Display(Name = "Email: ")]
	public string Email { get; set; } = null!;
}