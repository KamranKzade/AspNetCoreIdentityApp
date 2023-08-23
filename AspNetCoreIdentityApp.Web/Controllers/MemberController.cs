using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreIdentityApp.Web.ViewModels;

namespace AspNetCoreIdentityApp.Web.Controllers;

[Authorize]
public class MemberController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;

	public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
	{
		_signInManager = signInManager;
		_userManager = userManager;
	}


	public async Task<IActionResult> Index()
	{
		var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name);

		var userViewModel = new UserViewModel
		{
			UserName = currentUser.UserName,
			Email = currentUser.Email,
			PhoneNumber = currentUser.PhoneNumber
		};

		return View(userViewModel);
	}


	public async Task LogOut()
	{
		await _signInManager.SignOutAsync();
		// return RedirectToAction("Index", "Home");
	}

	public IActionResult PasswordChange()
	{
		return View();
	}

}
