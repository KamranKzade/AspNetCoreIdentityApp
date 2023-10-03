namespace AspNetCoreIdentityApp.Service.Services.Abstract;

public interface IEmailService
{
    Task SendResetPasswordEmail(string resetPasswordEmailLink, string ToEmail);
}
