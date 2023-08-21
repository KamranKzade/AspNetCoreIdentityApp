using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Models;

namespace AspNetCoreIdentityApp.Web.Controllers;

public class MemberController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;


	public MemberController(SignInManager<AppUser> signInManager)
	{
		_signInManager = signInManager;
	}

	public async Task LogOut()
	{
		await _signInManager.SignOutAsync();
		// return RedirectToAction("Index", "Home");
	}

}
