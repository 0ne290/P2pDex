/*using System.Security.Claims;
using Core.Application.General.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/auth")]
[Authorize]
public class AuthenticationController : Controller
{
    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Route("{userId:long}")]
    [HttpGet]
    public async Task<IActionResult> Handle(long userId)
    {
        var result = await _mediator.Send(new EnsureExistedOfTraderCommand { Id = userId});
        
        var claims = new[] { new Claim(ClaimTypes.Name, userId.ToString()), new Claim(ClaimTypes.Role, "User") };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            IsPersistent = true
        };

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
            authProperties);

        return ActionResultHelper.CreateResponse(result, HttpContext);
    }
    
    [Authorize]
    [Route("check")]
    [HttpGet]
    public IActionResult Check()
    {
        return Ok();
    }

    private readonly IMediator _mediator;
}*/