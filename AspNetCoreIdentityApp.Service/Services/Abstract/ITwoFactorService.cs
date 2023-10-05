namespace AspNetCoreIdentityApp.Service.Services.Abstract;

public interface ITwoFactorService
{
	public string GenerateGrCodeUri(string email, string unformattedKey);
	public int GetCodeVerification();
}
