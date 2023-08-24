using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly RoleManager<AppRole> _roleManager;

	public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
	{
		_userManager = userManager;
		_roleManager = roleManager;
	}

	public IActionResult Index()
	{
		return View();
	}
}
