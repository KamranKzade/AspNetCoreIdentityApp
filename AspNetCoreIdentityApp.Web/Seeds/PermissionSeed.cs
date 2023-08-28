using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.PermissionsRoot;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Seeds;

public class PermissionSeed
{
	public static async Task Seed(RoleManager<AppRole> roleManager)
	{
		var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
		var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvencedRole");
		var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

		if (!hasBasicRole)
		{
			await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });

			var basicRole = await roleManager.FindByNameAsync("BasicRole");

			await AddReadPermission(roleManager, basicRole);
		}

		if (!hasAdvancedRole)
		{
			await roleManager.CreateAsync(new AppRole() { Name = "AdvencedRole" });

			var basicRole = await roleManager.FindByNameAsync("AdvencedRole");

			await AddReadPermission(roleManager, basicRole);
			await AddUpdateAndCreatePermission(roleManager, basicRole);
		}

		if (!hasAdminRole)
		{
			await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" });

			var basicRole = await roleManager.FindByNameAsync("AdminRole");

			await AddReadPermission(roleManager, basicRole);
			await AddUpdateAndCreatePermission(roleManager, basicRole);
			await AddDeletePermission(roleManager, basicRole);
		}

	}




	private static async Task AddReadPermission(RoleManager<AppRole> roleManager, AppRole basicRole)
	{
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Stock.Read));
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Order.Read));
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Catalog.Read));
	}

	private static async Task AddUpdateAndCreatePermission(RoleManager<AppRole> roleManager, AppRole basicRole)
	{
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Stock.Create));
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Order.Create));
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Catalog.Create));


		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Stock.Update));
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Order.Update));
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Catalog.Update));

	}

	private static async Task AddDeletePermission(RoleManager<AppRole> roleManager, AppRole basicRole)
	{
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Stock.Delete));
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Order.Delete));
		await roleManager.AddClaimAsync(basicRole, new("Permission", Permissions.Catalog.Delete));
	}

}