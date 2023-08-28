using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
	public interface IMemberService
	{
		Task<UserManager> GetUserViewModelByUserNameAsync(string username);
		Task LogOutAsync();
		Task<bool> CheckPasswordAsync(string username, string password);
		Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword);


	}
}
