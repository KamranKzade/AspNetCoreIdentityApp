using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Service.Services;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.Models;

namespace AspNetCoreIdentityApp.Web.Controllers;

[Authorize]
public class MemberController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;
	private readonly IFileProvider _fileProvider;
	private readonly IMemberService _memberService;
	private string userName => User.Identity!.Name!;


	public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_fileProvider = fileProvider;
		_memberService = memberService;
	}


	public async Task<IActionResult> Index()
	{
		return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
	}

	public async Task LogOut()
	{
		await _memberService.LogOutAsync();
	}

	public IActionResult PasswordChange()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
	{
		if (!ModelState.IsValid)
			return View();

		if (!await _memberService.CheckPasswordAsync(userName, request.PasswordOld))
			ModelState.AddModelError(string.Empty, "Eski şifrənin yanlışdır");

		var (isSuccess, errors) = await _memberService.ChangePasswordAsync(userName, request.PasswordOld, request.PasswordNew);

		if (!isSuccess)
		{
			ModelState.AddModelErrorList(errors!);
			return View();
		}

		TempData["SuccessMessage"] = "Şifrənin başarı ilə dəyişilmişdir";

		return View();
	}

	public async Task<IActionResult> UserEdit()
	{
		ViewBag.genderList = _memberService.GetGenderSelectList();

		return View(await _memberService.GetUserEditViewModelAsync(userName));
	}

	[HttpPost]
	public async Task<IActionResult> UserEdit(UserEditViewModel request)
	{
		if (!ModelState.IsValid)
			return View();

		var (isSuccess, errors) = await _memberService.EditUserAsync(request, userName);


		if (!isSuccess)
		{
			ModelState.AddModelErrorList(errors);
			return View();
		}

		TempData["SuccessMessage"] = "İstidadəçi bilgiləri başarı ilə dəyişdirilmişdir";

		return View(await _memberService.GetUserEditViewModelAsync(userName));
	}

	public IActionResult AccessDenied(string ReturnUrl)
	{
		string message = string.Empty;

		message = "Bu sayfanı görməyə icazəniz yoxdur. Icazə almaq üçün yönəticiniz ilə görüşün";
		ViewBag.message = message;


		return View();
	}

	[HttpGet]
	public IActionResult Claims()
	{
		return View(_memberService.GetClaims(User));
	}


	[Authorize(Policy = "BakuPolicy")]
	[HttpGet]
	public IActionResult BakuPage()
	{
		return View();
	}


	[Authorize(Policy = "ExchangePolicy")]
	[HttpGet]
	public IActionResult ExchangePage()
	{
		return View();
	}


	[Authorize(Policy = "ViolencePolicy")]
	[HttpGet]
	public IActionResult ViolencePage()
	{
		return View();
	}

	public IActionResult TwoFactorAuth()
	{
		return View(new AuthenticatorViewModel() { TwoFactorType = (TwoFactor)_userManager.FindByNameAsync(userName).Result.TwoFactor! });
	}


}