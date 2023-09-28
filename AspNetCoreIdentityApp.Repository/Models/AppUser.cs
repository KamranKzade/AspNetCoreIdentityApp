using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Core.Models;

namespace AspNetCoreIdentityApp.Repository.Models;

public class AppUser : IdentityUser
{
    public string? City { get; set; }
    public string? Picture { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
}
