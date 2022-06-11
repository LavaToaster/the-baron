using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TheBaron.Http.Extensions;

namespace TheBaron.Http.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    [HttpGet("~/signin")]
    public async Task<IActionResult> SignIn()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = Url.Action("Select", "Discord") }, "Discord");
    }

    [HttpPost("~/signin")]
    public async Task<IActionResult> SignIn([FromForm] string provider)
    {
        // Note: the "provider" parameter corresponds to the external
        // authentication provider chosen by the user agent.
        if (string.IsNullOrWhiteSpace(provider)) return BadRequest();

        if (!await HttpContext.IsProviderSupportedAsync(provider)) return BadRequest();

        // Instruct the middleware corresponding to the requested external identity
        // provider to redirect the user agent to its own authorization endpoint.
        // Note: the authenticationScheme parameter must match the value configured in Startup.cs
        return Challenge(new AuthenticationProperties { RedirectUri = Url.Action("Select", "Discord") }, provider);
    }

    [HttpGet("~/signout")]
    [HttpPost("~/signout")]
    public async Task<IActionResult> SignOutCurrentUser()
    {
        await SignOut(new AuthenticationProperties { RedirectUri = Url.Action("SignIn") },
            CookieAuthenticationDefaults.AuthenticationScheme).ExecuteResultAsync(new ActionContext
        {
            HttpContext = HttpContext
        });
        
        return RedirectToAction("SignIn");
    }
}