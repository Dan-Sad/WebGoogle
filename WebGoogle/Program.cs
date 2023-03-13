using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

string pathToBlogs = builder.Configuration["Blogs"] ?? string.Empty;
if (!string.IsNullOrEmpty(pathToBlogs) )
    builder.Services.AddSingleton(pathToBlogs);

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
        options.Events = new OAuthEvents() { OnCreatingTicket = async context => await TokenProcessing(context) };
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

static async Task TokenProcessing(OAuthCreatingTicketContext context)
{
    var accessToken = context.AccessToken;
    var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
    var userInfo = JObject.Parse(await response.Content.ReadAsStringAsync());

    string id = userInfo.Value<string>("id");
    string email = userInfo.Value<string>("email");

    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
    {
        context.Fail("Failed to retrieve user ID or email from Google.");
        return;
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, id),
        new Claim(ClaimTypes.Email, email),
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
}