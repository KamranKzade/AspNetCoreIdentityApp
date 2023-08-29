using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
	public class MemberService : IMemberService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IFileProvider _fileProvider;
		public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_fileProvider = fileProvider;
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


		public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
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

		public async Task<UserEditViewModel> GetUserEditViewModelAsync(string username)
		{
			var currentUser = await _userManager.FindByNameAsync(username);

			return new UserEditViewModel()
			{
				Username = currentUser.UserName,
				Email = currentUser.Email,
				Phone = currentUser.PhoneNumber,
				BirtDate = currentUser.BirthDate,
				City = currentUser.City,
				Gender = currentUser.Gender,
			};
		}

		public SelectList GetGenderSelectList()
		{
			return new SelectList(Enum.GetNames(typeof(Gender)));
		}

		public async Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(UserEditViewModel request, string userName)
		{
			var currentUser = await _userManager.FindByNameAsync(userName);

			currentUser.UserName = request.Username;
			currentUser.Email = request.Email;
			currentUser.PhoneNumber = request.Phone;
			currentUser.Gender = request.Gender;
			currentUser.BirthDate = request.BirtDate;
			currentUser.City = request.City;


			if (request.Picture != null && request.Picture.Length > 0)
			{
				// wwwroot folderini aliriq
				var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
				// ozumuzden random file name yaradiriq, sekile vermek ucun
				var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";
				// requestden gelen sekil ucun yeni pathi veririk
				var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath, randomFileName);

				// Sekili path-e uygun olaraq kayd edirik
				using var stream = new FileStream(newPicturePath, FileMode.Create);
				await request.Picture.CopyToAsync(stream);


				currentUser.Picture = randomFileName;
			}


			var updateToUserResult = await _userManager.UpdateAsync(currentUser);

			if (!updateToUserResult.Succeeded)
				return (false, updateToUserResult.Errors);

			await _userManager.UpdateSecurityStampAsync(currentUser);
			await _signInManager.SignOutAsync();

			if (request.BirtDate.HasValue)
			{
				await _signInManager.SignInWithClaimsAsync(user: currentUser, isPersistent: true, additionalClaims: new[]
				{
						new Claim("birhdate", currentUser.BirthDate!.Value.ToString())
				});
			}
			else
				await _signInManager.SignInAsync(currentUser, isPersistent: true);

			return (true, null);
		}

		public List<ClaimViewModel> GetClaims(ClaimsPrincipal principal)
		{
			return principal.Claims.Select(x => new ClaimViewModel()
			{
				Issuer = x.Issuer,
				Type = x.Type,
				Value = x.Value
			}).ToList();
		}

		public async Task<(bool, IEnumerable<IdentityError>?)> SignUpAsync(SignUpViewModel request)
		{
			var identityResult = await _userManager.CreateAsync(new AppUser
			{
				UserName = request.Username,
				PhoneNumber = request.Phone,
				Email = request.Email
			},
			request.PasswordConfirm);

			if (!identityResult.Succeeded)
			{
				// identity ugurla olmasa, bu zaman artiq erroru ekrana cixaririq. Extenstion method ile yazdiq
				return (false, identityResult.Errors);
			}

			return (true, null);
		}

		public async Task<(bool, IEnumerable<IdentityError>?)> SignUpWithClaimAsync(SignUpViewModel request)
		{
			// Claim yaratdig, hansi ki, qeydiyyatdan kecen user 10 gir elave istifade ede bilsin deye
			var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
			// Cari useri tapdiq, bize gelen user uzerinden 
			var currentUser = await _userManager.FindByNameAsync(request.Username);
			// Daha sonra UserClaim cedveline bu useri ve claimi elave etdik
			var claimResult = await _userManager.AddClaimAsync(currentUser, exchangeExpireClaim);

			if (!claimResult.Succeeded)
				return (false, claimResult.Errors);

			return (true, null);
		}

		public async Task<(AppUser, IEnumerable<IdentityError>?)> CheckUserAsync(string email)
		{
			var hasUser = await _userManager.FindByEmailAsync(email);

			if (hasUser == null)
			{
				//ModelState.AddModelError(string.Empty, "Bu email adressinə sahib kullanıcı bulunamamışdır.");
				return (null!, new List<IdentityError>
				{
					new IdentityError
					{
						Code = string.Empty,
						Description = "Bu email adressinə sahib kullanıcı bulunamamışdır."
					}
				});
			}
			return (hasUser, null);
		}



	}
}
