using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace WebGoogle.Services
{
	public class GoogleAuthenticationEvents : OAuthEvents
	{
		public override async Task CreatingTicket(OAuthCreatingTicketContext context)
		{
			var accessToken = context.AccessToken;
			if (accessToken == null) throw new Exception("AccessToken is null.");

			var claimsPrincipal = await GetClaimsPrincipalAsync(accessToken);
			await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
		}

		public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string accessToken)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, Constants.GoogleUserInfoUrl);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			var httpClient = new HttpClient();
			var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to get user info from Google. Status code: {response.StatusCode}");
			}

			var userInfo = JObject.Parse(await response.Content.ReadAsStringAsync());
			string id = userInfo.Value<string>("id");
			string email = userInfo.Value<string>("email");

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
			{
				throw new Exception("Failed to retrieve user ID or email from Google.");
			}

			var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, id),
			new Claim(ClaimTypes.Email, email),
		};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

			return new ClaimsPrincipal(claimsIdentity);
		}
	}

}
