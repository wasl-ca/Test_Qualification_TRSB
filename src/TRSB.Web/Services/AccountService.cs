using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TRSB.Application.Common;
using TRSB.Web.Models;
using TRSB.Web.Services.Interfaces;

public class AccountService : IAccountService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _context;
    private readonly ILogger<AccountService> _logger;

    public AccountService(HttpClient http, IHttpContextAccessor context, ILogger<AccountService> logger)
    {
        _http = http;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<LoginResponse?>> Login(LoginViewModel model)
    {
        var response = await _http.PostAsJsonAsync(
            "api/users/login",
            model);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Echec de connexion pour {UsernameOrEmail}: {StatusCode}",
                model.UsernameOrEmail, response.StatusCode);
            return Result<LoginResponse?>.Failure("Identifiants invalides");
        }

        _context.HttpContext!.Response.Cookies.Append(
         "access_token",
         loginResponse!.Token,
         new CookieOptions
         {
             HttpOnly = true,
             Secure = true,
             SameSite = SameSiteMode.Strict,
             Expires = DateTimeOffset.UtcNow.AddHours(24)
         }
        );
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, loginResponse.UserId.ToString()),
            new Claim("JWT", loginResponse.Token)
        };

        if (!string.IsNullOrEmpty(loginResponse.Username))
        {
            claims.Add(new Claim(ClaimTypes.Name, loginResponse.Username));
        }

        if (!string.IsNullOrEmpty(loginResponse.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, loginResponse.Email));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await _context.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24),
                AllowRefresh = true
            });

        _logger.LogInformation("Utilisateur {UsernameOrEmail} connecté avec succès", model.UsernameOrEmail);

        return Result<LoginResponse?>.Success(loginResponse);
    }

    public async Task<Result<bool>> Logout()
    {
        var httpContext = _context.HttpContext;
        if (httpContext == null)
        {
            _logger.LogError("HttpContext est null lors de la déconnexion");
            return Result<bool>.Failure("Erreur de contexte");
        }

        try
        {
            httpContext.Response.Cookies.Delete("access_token", new CookieOptions
            {
                Path = "/",
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });

           
            // ✅ 4. Déconnexion de la session Cookie Authentication
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Utilisateur déconnecté avec succès");

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la déconnexion");
            return Result<bool>.Failure("Erreur lors de la déconnexion");
        }
    }

    public async Task<Result<bool>> Register(RegisterViewModel model)
    {
        if (model.Password != model.ConfirmPassword)
        {
            _logger.LogWarning("Echec d'enregistrement pour {Email}: les mots de passe ne correspondent pas",
                model.Email);
            return Result<bool>.Failure("Les mots de passe ne correspondent pas");
        }

        var response = await _http.PostAsJsonAsync(
            "api/users",
            new {
                model.Username,
                model.Name,
                model.Email,
                model.Password
            });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Echec d'enregistrement pour {Email}: {StatusCode} - {Error}",
                model.Email, response.StatusCode, error);
            return Result<bool>.Failure($"Erreur lors de l'enregistrement: {error}");
        }

        _logger.LogInformation("Utilisateur {Email} enregistré avec succès", model.Email);
        return Result<bool>.Success(true);
    }
    public async Task<Result<ProfileViewModel?>> GetProfileAsync()
    {
        setHeaders();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var response = await _http.GetAsync("api/users/profile", cts.Token);
        if (!response.IsSuccessStatusCode)
            return Result<ProfileViewModel?>.Failure("Erreur lors de la récupération du profil");

        var profile = await response.Content.ReadFromJsonAsync<ProfileViewModel>(
                cancellationToken: cts.Token);
        _logger.LogInformation("Profil utilisateur récupéré avec succès");
        return Result<ProfileViewModel?>.Success(profile);
    }

    public async Task<Result<bool>> UpdateProfileAsync(UpdateProfileViewModel model)
    {
        setHeaders();

        var response = await _http.PutAsJsonAsync("api/users/profile", model);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Erreur lors de la mise à jour du profil: {StatusCode} - {Error}",
                response.StatusCode, error);
            return Result<bool>.Failure($"Erreur lors de la mise à jour du profil: {error}");
        }

        return Result<bool>.Success(true);
    }

    private void setHeaders()
    {
        var token = _context.HttpContext!.Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Tentative d'accès au profil sans token d'authentification");
            if (string.IsNullOrEmpty(token))
                throw new Exception("Token d'authentification manquant");

        }
        _http.DefaultRequestHeaders.Authorization =
               new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}
