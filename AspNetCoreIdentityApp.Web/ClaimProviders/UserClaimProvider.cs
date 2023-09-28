using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using AspNetCoreIdentityApp.Repository.Models;

namespace AspNetCoreIdentityApp.Web.ClaimProviders;

public class UserClaimProvider : IClaimsTransformation
{
	private readonly UserManager<AppUser> _userManager;

	public UserClaimProvider(UserManager<AppUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
	{
		var identityUser = principal.Identity as ClaimsIdentity;

		var currentUser = await _userManager.FindByNameAsync(identityUser!.Name);

		if (currentUser != null)
		{
			if (string.IsNullOrEmpty(currentUser.City)) return principal;


			if (!principal.HasClaim(x => x.Type == "City"))
			{
				Claim cityClaim = new Claim("city", currentUser.City);

				identityUser.AddClaim(cityClaim);
			}
		}

		return principal;
	}
}
