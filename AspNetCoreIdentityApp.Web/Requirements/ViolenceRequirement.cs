using Microsoft.AspNetCore.Authorization;
using System;

namespace AspNetCoreIdentityApp.Web.Requirements;

public class ViolenceRequirement : IAuthorizationRequirement
{
	public int ThresholAge { get; set; }
}

public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
	{
		if (!context.User.HasClaim(x => x.Type == "birhdate"))
		{
			context.Fail();
			return Task.CompletedTask;
		}

		var birthDateClaim = context.User.FindFirst("birhdate");

		var toDay = DateTime.Now;
		var birthDate = Convert.ToDateTime(birthDateClaim.Value);
		var age = toDay.Year - birthDate.Year;

		if (birthDate > toDay.AddYears(-age)) age--;

		if (requirement.ThresholAge > age)
		{
			context.Fail();
			return Task.CompletedTask;
		}

		context.Succeed(requirement);
		return Task.CompletedTask;
	}
}