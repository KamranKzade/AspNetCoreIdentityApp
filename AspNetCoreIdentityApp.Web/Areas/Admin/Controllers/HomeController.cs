using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Web.Areas.Admin.Models;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers;

[Authorize(Roles ="admin")]
[Area("Admin")]
public class HomeController : Controller
{
	private readonly UserManager<AppUser> _userManager;

	public HomeController(UserManager<AppUser> userManager)
	{
		_userManager = userManager;
	}

	public IActionResult Index()
	{
		return View();
	}

	public async Task<IActionResult> UserList()
	{
		var userList = await _userManager.Users.ToListAsync();

		var userViewModelList = userList.Select(x => new UserViewModel
		{
			Id = x.Id,
			Name = x.UserName,
			Email = x.Email,
		}).ToList();

		return View(userViewModelList);
	}
}
