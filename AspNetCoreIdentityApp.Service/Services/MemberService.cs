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
	}
}
