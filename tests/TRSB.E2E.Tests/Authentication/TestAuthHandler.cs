using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // ✅ Vérifier si un header Authorization est présent
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var authHeader = Request.Headers["Authorization"].ToString();

        // ✅ Vérifier le format "Bearer token"
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        // ✅ Parser le token (format personnalisé pour les tests)
        // Format attendu : "test-token-{userId}-{username}-{email}"
        if (!token.StartsWith("test-token-"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid test token"));
        }

        var parts = token.Split('-');
        if (parts.Length < 5)
        {
            return Task.FromResult(AuthenticateResult.Fail("Malformed test token"));
        }

        var userId = parts[2];
        var username = parts[3];
        var email = parts[4];

        // ✅ Créer les claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim("sub", userId)
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}