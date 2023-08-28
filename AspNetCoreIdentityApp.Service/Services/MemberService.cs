using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
	public class MemberService : IMemberService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<UserManager> GetUserViewModelByUserNameAsync(string username)
		{
			var currentUser = await _userManager.FindByNameAsync(username);

			return new UserManager
			{
				UserName = currentUser.UserName,
				Email = currentUser.Email,
				PhoneNumber = currentUser.PhoneNumber,
				PictureUrl = currentUser.Picture
			};

		}

		public async Task LogOutAsync()
		{
			await _signInManager.SignOutAsync();
		}

		public async Task<bool> CheckPasswordAsync(string username, string password)
		{
			var currentUser = await _userManager.FindByNameAsync(username);

			return await _userManager.CheckPasswordAsync(currentUser, password);
		}


		async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
		{
			var currentUser = await _userManager.FindByNameAsync(userName);

			var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

			if (!resultChangePassword.Succeeded)
				return (false, resultChangePassword.Errors);

			await _userManager.UpdateSecurityStampAsync(currentUser);
			await _signInManager.SignOutAsync();
			await _signInManager.PasswordSignInAsync(currentUser, newPassword, isPersistent: true, lockoutOnFailure: false);

			return (true, null);
		}




	}
}
