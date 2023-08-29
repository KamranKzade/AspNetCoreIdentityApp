using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Seeds;
using Microsoft.Extensions.FileProviders;
using AspNetCoreIdentityApp.Web.Extentions;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.OptionsModels;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"), options =>
	{
		options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
	});
});

// Security Stamp-a interval vermek
builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
{
	opt.ValidationInterval = TimeSpan.FromMinutes(30);
});

// wwwroot folderinde ola Userpicture folderine gede bilek deye, ozumuze referance noqtesi olaraq islediyimiz proyekti secdik
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

// Burada biz framework-e basa saliriqki, hansisa 1 classin constructorunda
// IOptions<EmailSettings> gorsen, get datalari EmailSettingsden oxu
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Identittyni sisteme elave edirik
builder.Services.AddIdentityWithExtention();

// Scopelari sisteme tanidiriq
builder.Services.AddScopedWithExtention();

// Userlere aid olan Claimler
builder.Services.AddAuthorizationWithExtention();

// Cookie-ni appin Configuration-a tanitmaq
builder.Services.ConfigureApplicationCookieWithExtention();

// Facebook ile giris etmek ucun lazim olan configuration
builder.Services.AddAuthentication().AddFacebook(facebookoptions =>
{
	facebookoptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
	facebookoptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});


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
