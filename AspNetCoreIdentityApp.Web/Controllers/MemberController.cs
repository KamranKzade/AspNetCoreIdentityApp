using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Service.Services.Abstract;

namespace AspNetCoreIdentityApp.Web.Controllers;

[Authorize]
public class MemberController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;
	private readonly IFileProvider _fileProvider;
	private readonly IMemberService _memberService;
	private readonly ITwoFactorService _twoFactorService;

	private string userName => User.Identity!.Name!;


	public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService, ITwoFactorService twoFactorService)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_fileProvider = fileProvider;
		_memberService = memberService;
		_twoFactorService = twoFactorService;
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

	public async Task<IActionResult> TwoFactorWithAuthenticator()
	{
		var currentUser = await _userManager.FindByNameAsync(userName);
		var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(currentUser);

		if (string.IsNullOrEmpty(unformattedKey))
		{
			await _userManager.ResetAuthenticatorKeyAsync(currentUser);
			unformattedKey = await _userManager.GetAuthenticatorKeyAsync(currentUser);
		}

		AuthenticatorViewModel authenticatorViewModel = new();

		authenticatorViewModel.SharedKey = unformattedKey;
		authenticatorViewModel.AuthenticatorUri = _twoFactorService.GenerateGrCodeUri(currentUser.Email, unformattedKey);

		return View(authenticatorViewModel);
	}

	[HttpPost]
	public async Task<IActionResult> TwoFactorWithAuthenticator(AuthenticatorViewModel authenticatorVM)
	{
		var currentUser = await _userManager.FindByNameAsync(userName);
		var verificationCode = authenticatorVM.VerificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);

		var is2FATokenValid = await _userManager.VerifyTwoFactorTokenAsync(currentUser, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

		if (is2FATokenValid)
		{
			currentUser.TwoFactorEnabled = true;
			currentUser.TwoFactor = (sbyte)TwoFactor.MicrosoftGoogle;

			var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(currentUser, 5);

			TempData["recoveryCodes"] = recoveryCodes;
			TempData["message"] = "İki adımlı kimlik doğrulama tipiniz Microsoft/Google Authenticator olarak belirlenmişdir";

			return RedirectToAction("TwoFactorAuth");
		}
		else
		{
			ModelState.AddModelError("", "Girdiyiniz doğrulama kodu yanlışdır");
			return View(authenticatorVM);
		}



		return View();
	}

	public IActionResult TwoFactorAuth()
	{
		return View(new AuthenticatorViewModel() { TwoFactorType = (TwoFactor)_userManager.FindByNameAsync(userName).Result.TwoFactor! });
	}

	[HttpPost]
	public async Task<IActionResult> TwoFactorAuth(AuthenticatorViewModel authenticatorVM)
	{
		switch (authenticatorVM.TwoFactorType)
		{
			case TwoFactor.None:
				_userManager.FindByNameAsync(userName).Result.TwoFactorEnabled = false;
				_userManager.FindByNameAsync(userName).Result.TwoFactor = (sbyte)TwoFactor.None;
				TempData["message"] = "Iki adımlı kimlik doğrulama tipiniz heçbiri olarak belirlenmişdir";
				break;
			case TwoFactor.Email:
				break;
			case TwoFactor.Phone:
				break;
			case TwoFactor.MicrosoftGoogle:
				return RedirectToAction("TwoFactorWithAuthenticator");

		}

		await _userManager.UpdateAsync(_userManager.FindByNameAsync(userName).Result);

		return View(authenticatorVM);
	}

}