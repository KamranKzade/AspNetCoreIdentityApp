using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Controllers;

[Authorize]
public class MemberController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;


	public MemberController(SignInManager<AppUser> signInManager)
	{
		_signInManager = signInManager;
	}


	public IActionResult Index()
	{
		return View();
	}


	public async Task LogOut()
	{
		await _signInManager.SignOutAsync();
		// return RedirectToAction("Index", "Home");
	}

}
