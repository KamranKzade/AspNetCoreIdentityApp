using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;

namespace AspNetCoreIdentityApp.Service.Services
{
	public interface IMemberService
	{
		Task<UserManager> GetUserViewModelByUserNameAsync(string username);
		Task LogOutAsync();
		Task<bool> CheckPasswordAsync(string username, string password);
		Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
		Task<UserEditViewModel> GetUserEditViewModelAsync(string username);
		SelectList GetGenderSelectList();
		Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(UserEditViewModel request, string userName);
		List<ClaimViewModel> GetClaims(ClaimsPrincipal principal);
		Task<(bool, IEnumerable<IdentityError>?)> SignUpAsync(SignUpViewModel request);
		Task<(bool, IEnumerable<IdentityError>?)> SignUpWithClaimAsync(SignUpViewModel request);

		Task<(AppUser, IEnumerable<IdentityError>?)> CheckUserAsync(string email);
	}
}
