using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Service.Services;
using AspNetCoreIdentityApp.Repository.Models;

namespace AspNetCoreIdentityApp.Web.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	// Kullanici ile bagli butun isleri heyata keciren classdir
	private readonly UserManager<AppUser> _userManager;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly IEmailService _emailService;
	private readonly IMemberService _memberService;

	public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IMemberService memberService)
	{
		_logger = logger;
		_userManager = userManager;
		_signInManager = signInManager;
		_emailService = emailService;
		_memberService = memberService;
	}


	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[HttpGet]
	public IActionResult SignUp()
	{
		return View();
	}

	[HttpGet]
	public IActionResult SignIn()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> SignUp(SignUpViewModel request)
	{
		if (!ModelState.IsValid)
		{
			return View();
		}

		// identity-ni userManager ile yaratmaga calisiriq
		var (isSuccess, errors) = await _memberService.SignUpAsync(request);

		if (!isSuccess)
		{
			// identity ugurla olmasa, bu zaman artiq erroru ekrana cixaririq. Extenstion method ile yazdiq
			ModelState.AddModelErrorList(errors);
			return View();
		}

		var (isSuccessForClaim, errorsForClaim) = await _memberService.SignUpWithClaimAsync(request);

		if (!isSuccessForClaim)
		{
			// identity ugurla olmasa, bu zaman artiq erroru ekrana cixaririq. Extenstion method ile yazdiq
			ModelState.AddModelErrorList(errorsForClaim!);
			return View();
		}

		TempData["SuccessMessage"] = "Uyelik kayit islemi basarili ile gerceklemistir";
		return RedirectToAction(nameof(HomeController.SignUp));
	}


	[HttpPost]
	public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
	{
		if (!ModelState.IsValid)
			return View();

		returnUrl ??= Url.Action("Index", "Home");

		// Emaile gore useri axtaririq
		var hasUser = await _userManager.FindByEmailAsync(model.Email);

		// Eger user yoxdusa onda error messagi cixaririq ekrana
		if (hasUser == null)
		{
			ModelState.AddModelError(string.Empty, "Email veya sifre yanlisdir");
			return View();
		}

		// Eger varsa SignInManager uzerinden gelen PasswordSignInAsync metodu ile giris elemeye calisir,
		// yeni uygun username-e uygun parolunda duzgun olub olmadigini yoxlayiriq
		// hasUser				  --> emaile uygun gelen userdir
		// model.Password		  --> paroldur
		// model.RememberMe		  --> cookiede tutub, yazilan mail ve passwordu yadda saxlayib saxlamamaqdi ( bool ) 
		// lockoutOnFailure:true  --> nece girisden sonra hesabi mueyyen muddetlik kilidlemekdir ( bool )
		var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, lockoutOnFailure: true);


		if (signInResult.IsLockedOut)
		{
			ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giris yapamazsiniz." });
			return View();
		}

		if (!signInResult.Succeeded)
		{
			ModelState.AddModelErrorList(new List<string>() { "Email veya sifre yanlis", $"Basarisiz giris sayisi ={await _userManager.GetAccessFailedCountAsync(hasUser)}" });
			return View();
		}

		// Yoxlayiriqki, userin birtdate-i daxil edilib, eger daxil edilibse claimlere birtdate-e uygun value-nu yaziriq
		if (hasUser.BirthDate.HasValue)
		{
			await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe, new[]
			{
					new Claim("birhdate", hasUser.BirthDate.Value.ToString())
			});
		}

		return Redirect(returnUrl!);

	}

	public IActionResult ForgetPassword()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
	{
		var (hasUser,errors) = await _memberService.CheckUserAsync(request.Email);

		if (hasUser == null)
		{
			ModelState.AddModelErrorList(errors!);
			return View();
		}

		string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

		var passswordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

		// ornek link htpps://localhost:7111?userId=12213&token=asdasdsadasdasdasda

		// Email Service
		// Ilk google hesabinizi honetine gedib --> Security --> App passwords --> Yeni bir uygulama ve sifre olustururuq, 
		// hansi ki, google uzerinde mail gondere bilek deye

		await _emailService.SendResetPasswordEmail(passswordResetLink!, hasUser.Email);


		TempData["SuccessMessage"] = "Şifre yeniləmə linkiç e-posta adressinizə göndərilmişdir";

		return RedirectToAction(nameof(ForgetPassword));
	}

	public IActionResult ResetPassword(string userId, string token)
	{
		TempData["userId"] = userId;
		TempData["token"] = token;

		return View();
	}

	[HttpPost]
	public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
	{
		var userId = TempData["userId"];
		var token = TempData["token"];

		if (userId == null || token == null)
		{
			throw new Exception("Bir hata meydana gəldi");
		}

		var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);

		if (hasUser == null)
		{
			ModelState.AddModelError(string.Empty, "Kullanıcı bulunamamıştır");
			return View();
		}

		var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);

		if (result.Succeeded)
		{
			TempData["SuccessMessage"] = "Şifrenin başarı ilə yenilənmişdir";
		}
		else
		{
			ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
			return View();
		}


		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}