using AspNetCoreIdentityApp.Core.ViewModels;
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
	}
}
