using System.Text.Encodings.Web;
using AspNetCoreIdentityApp.Service.Services.Abstract;

namespace AspNetCoreIdentityApp.Service.Services.Concrete;

public class TwoFactorService : ITwoFactorService
{
	private readonly UrlEncoder _urlEncoder;

	public TwoFactorService(UrlEncoder urlEncoder)
	{
		_urlEncoder = urlEncoder;
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
}

