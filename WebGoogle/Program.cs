using Microsoft.AspNetCore.Authentication.Cookies;
using WebGoogle.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

string pathToBlogs = builder.Configuration["Blogs"] ?? string.Empty;
if (!string.IsNullOrEmpty(pathToBlogs) )
    builder.Services.AddSingleton(pathToBlogs);

builder.Services.AddSingleton<GoogleAuthenticationEvents>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
})
.AddCookie(options => options.LoginPath = "/api/google-login")
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("'ClientId' for Google authentication not found");
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("'ClientSecret' for Google authentication not found");
    options.EventsType = typeof(GoogleAuthenticationEvents);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 
app.MapRazorPages();

app.Run();