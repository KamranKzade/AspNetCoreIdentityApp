using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspNetCoreIdentityApp.Web.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	// Kullanici ile bagli butun isleri heyata keciren classdir
	private readonly UserManager<AppUser> _userManager;


	public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
	{
		_logger = logger;
		_userManager = userManager;
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

	[HttpPost]
	public async Task<IActionResult> SignUp(SignUpViewModel request)
	{
		if(!ModelState.IsValid)
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

		// identity ugurla olmasa, bu zaman artiq erroru ekrana cixaririq.
		foreach (IdentityError item in identityResult.Errors)
		{
			ModelState.AddModelError(string.Empty, item.Description);
		}

		return View();
	}


	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}