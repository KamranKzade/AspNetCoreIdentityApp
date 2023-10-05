namespace AspNetCoreIdentityApp.Core.Models;

public class TwoFactorOptions
{
	public string SendGrid_ApiKey { get; set; }
	public int CodeTimeExpire { get; set; }
}
