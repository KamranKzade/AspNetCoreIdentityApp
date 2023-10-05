using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Service.Services.Abstract;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AspNetCoreIdentityApp.Service.Services.Concrete;

public class EmailSender
{
	private readonly TwoFactorOptions _twoFactorOptions;
	private readonly ITwoFactorService _twoFactorService;

	public EmailSender(IOptions<TwoFactorOptions> options, ITwoFactorService twoFactorService)
	{
		_twoFactorOptions = options.Value;
		_twoFactorService = twoFactorService;
	}


	public string Send(string emailAddress)
	{
		string code = _twoFactorService.GetCodeVerification().ToString();

		Execute(emailAddress, code).Wait();

		return code;
	}

	private async Task Execute(string email, string code)
	{
		var client = new SendGridClient(_twoFactorOptions.SendGrid_ApiKey);
		var from = new EmailAddress("311.kamran@gmail.com");

		var subject = "İki adımlı kimlik doğrulama kodunuz";
		var to = new EmailAddress(email);

		var htmlContext = $"<h2>Sayta giriş etmək üçün lazım olan doğrulama kodunuz aşağıdadır</h2><h3>Kodunuz: {code}</h3>";
		var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContext);
		var responce = await client.SendEmailAsync(msg);
	}

}
