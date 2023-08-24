using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly RoleManager<AppRole> _roleManager;

	public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
	{
		_userManager = userManager;
		_roleManager = roleManager;
	}

	public async Task<IActionResult> Index()
	{
		var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
		{
			Id = x.Id,
			Name = x.Name
		}).ToListAsync();

		return View(roles);
	}

	[HttpGet]
	public IActionResult RoleCreate()
	{
		return View();
	}

	[HttpPost]
	public async Task<ActionResult> RoleCreate(RoleCreateViewModel request)
	{
		var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

		if (!result.Succeeded)
		{
			ModelState.AddModelErrorList(result.Errors);
		}

		TempData["SuccessMessage"] = "Role uğurla yaradılmışdır";
		return RedirectToAction(nameof(RolesController.Index));
	}


	public async Task<IActionResult> RoleUpdate(string id)
	{
		var roleToUpdate = await _roleManager.FindByIdAsync(id);

		if (roleToUpdate == null) throw new Exception("Güncəllənəcək role tapilmayib");

		return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate.Name });
	}

	[HttpPost]
	public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
	{
		var roletoUpdate = await _roleManager.FindByIdAsync(request.Id);

		if (roletoUpdate == null) throw new Exception("Güncəllənəcək role tapilmayib");

		roletoUpdate.Name = request.Name;

		await _roleManager.UpdateAsync(roletoUpdate);

		ViewData["SuccessMessage"] = "Role bilgisi güncəllənmişdir";
		return View();
	}


	public async Task<IActionResult> RoleDelete(string id)
	{

		var roletoDelete = await _roleManager.FindByIdAsync(id);

		if (roletoDelete == null) throw new Exception("Silinəcək role tapilmayib");

		var result = await _roleManager.DeleteAsync(roletoDelete);

		if (!result.Succeeded)
		{
			//ModelState.AddModelErrorList(result.Errors);
			throw new Exception(result.Errors.Select(x=>x.Description).First());
		}
		TempData["SuccessMessage"] = "Role uğurla silinmişdir";

		return RedirectToAction(nameof(RolesController.Index));
	}
}
