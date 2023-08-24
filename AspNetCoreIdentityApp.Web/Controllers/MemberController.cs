using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreIdentityApp.Web.ViewModels;
using AspNetCoreIdentityApp.Web.Extentions;
using Microsoft.AspNetCore.Mvc.Rendering;

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

	[HttpPost]
	public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
	{
		if (!ModelState.IsValid)
		{
			return View();
		}

		var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name);

		var chechOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.PasswordOld);

		if (!chechOldPassword)
		{
			ModelState.AddModelError(string.Empty, "Eski şifrənin yanlışdır");
		}

		var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.PasswordOld, request.PasswordNew);

		if (!resultChangePassword.Succeeded)
		{
			ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x => x.Description).ToList());
			return View();
		}


		await _userManager.UpdateSecurityStampAsync(currentUser);
		await _signInManager.SignOutAsync();
		await _signInManager.PasswordSignInAsync(currentUser, request.PasswordNew, isPersistent: true, lockoutOnFailure: false);

		TempData["SuccessMessage"] = "Şifrənin başarı ilə dəyişilmişdir";

		return View();
	}


	public async Task<IActionResult> UserEdit()
	{
		ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
		var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name);

		var userEditViewModel = new UserEditViewModel()
		{
			Username = currentUser.UserName,
			Email = currentUser.Email,
			Phone = currentUser.PhoneNumber,
			BirtDate = currentUser.BirthDate,
			City = currentUser.City,
			Gender = currentUser.Gender,
		};

		return View(userEditViewModel);
	}
}
