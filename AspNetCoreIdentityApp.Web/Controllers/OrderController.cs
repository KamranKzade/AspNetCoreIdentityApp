using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Controllers;

public class OrderController : Controller
{
	[Authorize(Policy = "Permissions.Order.Read")]
	public IActionResult Index()
	{
		return View();
	}
}
