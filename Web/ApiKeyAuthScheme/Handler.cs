using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Web.ApiKeyAuthScheme;

public class ApiKeyAuthSchemeHandler : AuthenticationHandler<ApiKeyAuthSchemeOptions>
{
    public ApiKeyAuthSchemeHandler(IOptionsMonitor<ApiKeyAuthSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers.Authorization.ToString();

        if (!authHeader.StartsWith("ApiKey "))
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));

        var apiKey = authHeader["ApiKey ".Length..];
        
        if (apiKey != Options.ApiKey)
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));

        var claims = new[] { new Claim(ClaimTypes.Name, "Frontend") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}