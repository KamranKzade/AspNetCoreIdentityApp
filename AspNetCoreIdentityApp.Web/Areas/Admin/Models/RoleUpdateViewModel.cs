using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models;

public class RoleUpdateViewModel
{
	public string Id { get; set; } = null!;

	[Required(ErrorMessage = "Role isim alanı boş bıraxılamaz.")]
	[Display(Name = "Role Ismi: ")]
	public string Name { get; set; }
}
