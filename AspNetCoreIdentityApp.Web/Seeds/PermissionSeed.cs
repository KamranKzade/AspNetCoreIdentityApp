using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.PermissionsRoot;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Seeds;

public  class PermissionSeed
{
	public static async Task Seed(RoleManager<AppRole> roleManager)
	{
		var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");

		if (!hasBasicRole)
		{
			await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });

			var basicRole = await roleManager.FindByNameAsync("BasicRole");

			await roleManager.AddClaimAsync(basicRole, new("Permission", Permission.Stock.Read));
			await roleManager.AddClaimAsync(basicRole, new("Permission", Permission.Order.Read));
			await roleManager.AddClaimAsync(basicRole, new("Permission", Permission.Catalog.Read));
		}


	}

}
