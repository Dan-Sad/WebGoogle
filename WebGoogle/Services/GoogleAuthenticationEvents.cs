using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace WebGoogle.Services
{
    public class GoogleAuthenticationEvents : OAuthEvents
    {
        private readonly GoogleAuthenticationService _googleAuthService;

        public GoogleAuthenticationEvents(GoogleAuthenticationService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        public override async Task CreatingTicket(OAuthCreatingTicketContext context)
        {
            var accessToken = context.AccessToken;
            if (accessToken == null) throw new Exception("AccessToken is null.");

            var claimsPrincipal = await _googleAuthService.GetClaimsPrincipalAsync(accessToken);
            await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }
    }

}
