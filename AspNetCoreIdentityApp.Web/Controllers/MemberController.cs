using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreIdentityApp.Web.ViewModels;
using AspNetCoreIdentityApp.Web.Extentions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

namespace AspNetCoreIdentityApp.Web.Controllers;

[Authorize]
public class MemberController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;
	private readonly IFileProvider _fileProvider;


	public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_fileProvider = fileProvider;
	}


	public async Task<IActionResult> Index()
	{
		var userClaims = User.Claims.ToList();
		var email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email);


		var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name);

		var userViewModel = new UserViewModel
		{
			UserName = currentUser.UserName,
			Email = currentUser.Email,
			PhoneNumber = currentUser.PhoneNumber,
			PictureUrl=currentUser.Picture
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
			ModelState.AddModelErrorList(resultChangePassword.Errors);
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


	[HttpPost]
	public async Task<IActionResult> UserEdit(UserEditViewModel request)
	{
		if (!ModelState.IsValid)
		{
			return View();
		}

		var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name);

		currentUser.UserName = request.Username;
		currentUser.Email = request.Email;
		currentUser.PhoneNumber = request.Phone;
		currentUser.Gender = request.Gender;
		currentUser.BirthDate = request.BirtDate;
		currentUser.City = request.City;



		if (request.Picture != null && request.Picture.Length > 0)
		{
			// wwwroot folderini aliriq
			var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
			// ozumuzden random file name yaradiriq, sekile vermek ucun
			var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";
			// requestden gelen sekil ucun yeni pathi veririk
			var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath, randomFileName);

			// Sekili path-e uygun olaraq kayd edirik
			using var stream = new FileStream(newPicturePath, FileMode.Create);
			await request.Picture.CopyToAsync(stream);


			currentUser.Picture = randomFileName;
		}

		var updateToUserResult = await _userManager.UpdateAsync(currentUser);

		if (!updateToUserResult.Succeeded)
		{
			ModelState.AddModelErrorList(updateToUserResult.Errors);
			return View();
		}

		await _userManager.UpdateSecurityStampAsync(currentUser);
		await _signInManager.SignOutAsync();
		await _signInManager.SignInAsync(currentUser, isPersistent: true);

		TempData["SuccessMessage"] = "İstidadəçi bilgiləri başarı ilə dəyişdirilmişdir";


		var userEditViewModel = new UserEditViewModel()
		{
			Username = currentUser.UserName,
			Email = currentUser.Email,
			Phone = currentUser.PhoneNumber,
			Gender = currentUser.Gender,
			BirtDate = currentUser.BirthDate,
			City = currentUser.City
		};

		return View(userEditViewModel);
	}

	public IActionResult AccessDenied(string ReturnUrl)
	{
		string message = string.Empty;

		message = "Bu sayfanı görməyə icazəniz yoxdur. Icazə almaq üçün yönəticiniz ilə görüşün";
		ViewBag.message=message;


		return View();
	}
}
