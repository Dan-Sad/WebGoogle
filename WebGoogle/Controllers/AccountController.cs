using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGoogle.Controllers
{
    [Route("/api")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        [Route("/google-login")]
        public IActionResult GoogleLogin()
        {
            return Challenge("Google");
        }

        [HttpGet]
        [Route("/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}