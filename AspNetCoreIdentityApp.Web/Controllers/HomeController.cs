using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Web.ViewModels;

namespace AspNetCoreIdentityApp.Web.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	// Kullanici ile bagli butun isleri heyata keciren classdir
	private readonly UserManager<AppUser> _userManager;
	private readonly SignInManager<AppUser> _signInManager;


	public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
	{
		_logger = logger;
		_userManager = userManager;
		_signInManager = signInManager;
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
		var identityResult = await _userManager.CreateAsync(new AppUser
		{
			UserName = request.Username,
			PhoneNumber = request.Phone,
			Email = request.Email
		},
		request.PasswordConfirm);

		// identity ugurla bas veribse ViewBag ile ekrana melumat otururuk.
		if (identityResult.Succeeded)
		{
			TempData["SuccessMessage"] = "Uyelik kayit islemi basarili ile gerceklemistir";
			return RedirectToAction(nameof(HomeController.SignUp));
		}

		// identity ugurla olmasa, bu zaman artiq erroru ekrana cixaririq. Extenstion method ile yazdiq
		ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

		return View();
	}


	[HttpPost]
	public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
	{
		returnUrl = returnUrl ?? Url.Action("Index", "Home");

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

		if (signInResult.Succeeded)
		{
			return Redirect(returnUrl!);
		}

		if (signInResult.IsLockedOut)
		{
			ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giris yapamazsiniz." });
			return View();
		}

		ModelState.AddModelErrorList(new List<string>() { "Email veya sifre yanlis", $"Basarisiz giris sayisi ={await _userManager.GetAccessFailedCountAsync(hasUser)}" });

		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}