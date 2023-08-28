using AspNetCoreIdentityApp.Core.OptionsModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AspNetCoreIdentityApp.Web.Services;

public class EmailService : IEmailService
{
	private readonly EmailSettings _emailSettings;

	public EmailService(IOptions<EmailSettings> options)
	{
		_emailSettings = options.Value;
	}


	public async Task SendResetPasswordEmail(string resetEmailLink, string ToEmail)
	{
		// Messagi gondermek ucun lazim olan protokol
		var smtpClient = new SmtpClient();

		// Baglanti novunu secirik ve elave ozellikleri veririk
		smtpClient.Host = _emailSettings.Host;
		smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
		smtpClient.UseDefaultCredentials = false;
		smtpClient.Port = 587; // Portu belirtdik
		smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password); // Credential-lari belirtdik
		smtpClient.EnableSsl = true;


		var mailMessage = new MailMessage();
		mailMessage.From = new MailAddress(_emailSettings.Email); // Kimden Email gedir, onu deyirik
		mailMessage.To.Add(ToEmail); // Kime gedecek, onu deyirik

		// Subject ve body -ni yaziriq
		mailMessage.Subject = "Localhost | Şifre sıfırlama linki"; 
		mailMessage.Body = @$"<h4>Şifrenizi yeniləmək üçün aşağıdakı linkə girin.</h4>
				<p>
					<a href='{resetEmailLink}'>Şifre yeniləmə link</a>
				</p>";

		mailMessage.IsBodyHtml = true;
		await smtpClient.SendMailAsync(mailMessage);
	}
}
