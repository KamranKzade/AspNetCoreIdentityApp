﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Web.Extentions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Service.Services;

namespace AspNetCoreIdentityApp.Web.Controllers;

[Authorize]
public class MemberController : Controller
{
	private readonly SignInManager<AppUser> _signInManager;
	private readonly UserManager<AppUser> _userManager;
	private readonly IFileProvider _fileProvider;
	private readonly IMemberService _memberService;


	private string userName => User.Identity!.Name!;


	public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_fileProvider = fileProvider;
		_memberService = memberService;
	}


	public async Task<IActionResult> Index()
	{
		return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
	}


	public async Task LogOut()
	{
		await _memberService.LogOutAsync();
	}

	public IActionResult PasswordChange()
	{
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
	{
		if (!ModelState.IsValid)
		{
			return View();
		}

		var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name);

		var chechOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.PasswordOld);

		if (!chechOldPassword)
		{
			ModelState.AddModelError(string.Empty, "Eski şifrənin yanlışdır");
		}

		var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.PasswordOld, request.PasswordNew);

		if (!resultChangePassword.Succeeded)
		{
			ModelState.AddModelErrorList(resultChangePassword.Errors);
			return View();
		}


		await _userManager.UpdateSecurityStampAsync(currentUser);
		await _signInManager.SignOutAsync();
		await _signInManager.PasswordSignInAsync(currentUser, request.PasswordNew, isPersistent: true, lockoutOnFailure: false);

		TempData["SuccessMessage"] = "Şifrənin başarı ilə dəyişilmişdir";

		return View();
	}


	public async Task<IActionResult> UserEdit()
	{
		ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
		var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name);

		var userEditViewModel = new UserEditViewModel()
		{
			Username = currentUser.UserName,
			Email = currentUser.Email,
			Phone = currentUser.PhoneNumber,
			BirtDate = currentUser.BirthDate,
			City = currentUser.City,
			Gender = currentUser.Gender,
		};

		return View(userEditViewModel);
	}


	[HttpPost]
	public async Task<IActionResult> UserEdit(UserEditViewModel request)
	{
		if (!ModelState.IsValid)
		{
			return View();
		}

		var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name);

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
		{
			ModelState.AddModelErrorList(updateToUserResult.Errors);
			return View();
		}

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
		{
			await _signInManager.SignInAsync(currentUser, isPersistent: true);
		}

		TempData["SuccessMessage"] = "İstidadəçi bilgiləri başarı ilə dəyişdirilmişdir";


		var userEditViewModel = new UserEditViewModel()
		{
			Username = currentUser.UserName,
			Email = currentUser.Email,
			Phone = currentUser.PhoneNumber,
			Gender = currentUser.Gender,
			BirtDate = currentUser.BirthDate,
			City = currentUser.City
		};

		return View(userEditViewModel);
	}

	public IActionResult AccessDenied(string ReturnUrl)
	{
		string message = string.Empty;

		message = "Bu sayfanı görməyə icazəniz yoxdur. Icazə almaq üçün yönəticiniz ilə görüşün";
		ViewBag.message = message;


		return View();
	}


	[HttpGet]
	public IActionResult Claims()
	{
		var userClaimList = User.Claims.Select(x => new ClaimViewModel()
		{
			Issuer = x.Issuer,
			Type = x.Type,
			Value = x.Value
		}).ToList();

		return View(userClaimList);
	}


	[Authorize(Policy = "BakuPolicy")]
	[HttpGet]
	public IActionResult BakuPage()
	{
		return View();
	}


	[Authorize(Policy = "ExchangePolicy")]
	[HttpGet]
	public IActionResult ExchangePage()
	{
		return View();
	}


	[Authorize(Policy = "ViolencePolicy")]
	[HttpGet]
	public IActionResult ViolencePage()
	{
		return View();
	}

}
