using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Seeds;
using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Repository.Models;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContextWithExtention(builder.Configuration);
builder.Services.ConfigureWithExtention();
builder.Services.AddSingletonWithExtention();
builder.Services.ConfigureWithExtentionForEmailService(builder.Configuration);
builder.Services.AddIdentityWithExtention();
builder.Services.AddScopedWithExtention();
builder.Services.AddAuthorizationWithExtention();
builder.Services.ConfigureApplicationCookieWithExtention();
builder.Services.AddAuthenticationWithExtention(builder.Configuration);


var app = builder.Build();

// Permisionlari bura qeyd edirik
using (var scope = app.Services.CreateScope())
{
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

	await PermissionSeed.Seed(roleManager);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Area-lar ile islemek ucun
app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
