using System.Text.Encodings.Web;
using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Service.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AspNetCoreIdentityApp.Service.Services.Concrete;

public class TwoFactorService : ITwoFactorService
{
	private readonly UrlEncoder _urlEncoder;
	private readonly TwoFactorOptions _twoFactorOptions;

	public TwoFactorService(UrlEncoder urlEncoder, IOptions<TwoFactorOptions> options)
	{
		_urlEncoder = urlEncoder;
		_twoFactorOptions = options.Value;
	}

	public string GenerateGrCodeUri(string email, string unformattedKey)
	{
		const string format = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

		return string.Format(format, _urlEncoder.Encode("www.aspnetcoreidentity.com"), _urlEncoder.Encode(email), unformattedKey);
	}

	public int GetCodeVerification()
	{
		Random rnd = new Random();
		return rnd.Next(1000, 9999);
	}

	public int TimeLeft(HttpContext context)
	{
		if (context.Session.GetString("currentTime") == null)
		{
			context.Session.SetString("currentTime", DateTime.Now.AddSeconds(_twoFactorOptions.CodeTimeExpire).ToString());
		}

		var currentTime = DateTime.Parse(context.Session.GetString("currentTime")!.ToString());

		int timeLeft = (int)(currentTime - DateTime.Now).TotalSeconds;

		if (timeLeft <= 0)
		{
			context.Session.Remove("currentTime");
			return 0;
		}
		else
			return timeLeft;
	}

}

