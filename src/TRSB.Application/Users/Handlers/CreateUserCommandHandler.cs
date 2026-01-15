using MediatR;
using TRSB.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using TRSB.Domain.ValueObjects;
using TRSB.Application.Users.Commands;
using TRSB.Application.Common;

namespace TRSB.Application.Users.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordPolicy _passwordPolicy;
        private readonly IPasswordHasher<User> _passwordHasher;

        public CreateUserCommandHandler(IRepository<User> userRepository, IPasswordPolicy passwordPolicy, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordPolicy = passwordPolicy;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Null ou vide pour username, name, email et password
            if (string.IsNullOrWhiteSpace(request.Username))
                return Result<Guid>.Failure("Le nom d'usager est requis");  

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<Guid>.Failure("Le nom est requis");   

            if (string.IsNullOrWhiteSpace(request.Email))
                return Result<Guid>.Failure("Le courriel est requis");

            if (string.IsNullOrWhiteSpace(request.Password))
                return Result<Guid>.Failure("Le mot de passe est requis");

            // Vérifier politique de mot de passe
            if (!await _passwordPolicy.ValidateAsync(request.Password))
                return Result<Guid>.Failure("Le mot de passe ne respecte pas les exigences de la politique");

            // Vérifier unicité Nom usager
            if (await _userRepository.ExistsAsync(u => u.Username == request.Username))
                return Result<Guid>.Failure("Le nom d'usager existe déjà");

            // Vérifier unicité Courriel 
            if (await _userRepository.ExistsAsync(u => u.EmailValue == request.Email))
                return Result<Guid>.Failure("Le courriel existe déjà");

            try
            {
                // Hash du mot de passe
                var user = new User
                (
                    request.Username,
                    request.Name,
                    new Email(request.Email)
                );

                var passwordHash = _passwordHasher.HashPassword(user, request.Password);

                user.SetPasswordHash(passwordHash);

                // Ajouter et sauvegarder
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                return Result<Guid>.Success(user.Id);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
            
        }
    }
}
