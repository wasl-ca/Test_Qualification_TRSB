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

    public async Task<Result<string?>> Login(LoginViewModel model)
    {
        var response = await _http.PostAsJsonAsync(
            "api/users/login",
            model);

        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Echec de connexion pour {UsernameOrEmail}: {StatusCode}",
                model.UsernameOrEmail, response.StatusCode);
            return Result<string?>.Failure(json ?? "Identifiants invalides");
        }

        var token = JsonDocument.Parse(json)
            .RootElement.GetProperty("token").GetString();
        _logger.LogInformation("Utilisateur {UsernameOrEmail} connecté avec succès",
            model.UsernameOrEmail);
        _context.HttpContext!.Response.Cookies.Append(
         "access_token",
         token!,
         new CookieOptions { HttpOnly = true }
        );
        var claims = new List<Claim>
        {
            new Claim("JWT", token!)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await _context.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);
        return Result<string?>.Success(token);
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
            "api/users/create",
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
        setAuthenticated();

        var response = await _http.GetAsync("api/users/profile");
        if (!response.IsSuccessStatusCode)
            return Result<ProfileViewModel?>.Failure("Erreur lors de la récupération du profil");

        var json = await response.Content.ReadAsStringAsync();
        return Result<ProfileViewModel?>.Success(JsonSerializer.Deserialize<ProfileViewModel>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
    }

    public async Task<Result<bool>> UpdateProfileAsync(UpdateProfileViewModel model)
    {
        setAuthenticated();

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

    private void setAuthenticated()
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
