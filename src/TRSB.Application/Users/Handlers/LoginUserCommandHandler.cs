using MediatR;
using TRSB.Domain.Interfaces;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using TRSB.Application.Users.Commands;
using TRSB.Application.Common;

namespace TRSB.Application.Users.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;

        public LoginUserCommandHandler(IRepository<User> userRepository, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // Validation des entrées
            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
                return Result<string>.Failure("Le nom d'usager ou le courriel est requis");
            if (string.IsNullOrWhiteSpace(request.Password))
                return Result<string>.Failure("Le mot de passe est requis");

            // Récupérer user par username ou email
            var user = (await _userRepository.FindAsync(u =>
                u.Username == request.UsernameOrEmail || u.EmailValue == request.UsernameOrEmail))
                .FirstOrDefault();

            if (user == null)
                return Result<string>.Failure("Identifiants invalides");

            // Vérifier le mot de passe
            if (!_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password).Equals(PasswordVerificationResult.Success))
                return Result<string>.Failure("Identifiants invalides");

            // Générer un JWT simple
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:SigningKey") ?? throw new InvalidOperationException("JWT Key not configured"));
            var issuer = _configuration.GetValue<string>("Jwt:Issuer") ?? throw new InvalidOperationException("JWT Issuer not configured");
            var audience = _configuration.GetValue<string>("Jwt:Audience") ?? throw new InvalidOperationException("JWT Audience not configured");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow. ToUnixTimeSeconds().ToString()),
                    
                    // ASP.NET Core Identity claims
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username ??  string.Empty),
                    new Claim(ClaimTypes.Email, user.EmailValue ?? string. Empty),
                }),
                Issuer = issuer,
                Audience = audience,
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpireMinutes") > 0 ? _configuration.GetValue<int>("Jwt:ExpireMinutes") : 60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Result<string>.Success(tokenHandler.WriteToken(token));
        }
    }
}
