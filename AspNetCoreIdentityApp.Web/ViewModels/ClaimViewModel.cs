namespace AspNetCoreIdentityApp.Web.ViewModels;

public class ClaimViewModel
{

    // Bize claimler fb, google danda gele biler deye, issuer ile kimin terefinden geldiyinide saxlayiriq
    public string Issuer { get; set; } = null!;
    public string Type { get; set; } = null!;
	public string Value { get; set; } = null!;
}
