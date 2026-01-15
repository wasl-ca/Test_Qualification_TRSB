using MediatR;
using TRSB.Application.Common;
using TRSB.Application.Users.Commands;
using TRSB.Domain.Entities;
using TRSB.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace TRSB.Application.Users.Handlers
{

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<bool>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IPasswordPolicy _passwordPolicy;

        public UpdateUserCommandHandler(IRepository<User> userRepository, IPasswordHasher<User> passwordHasher, IPasswordPolicy passwordPolicy)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _passwordPolicy = passwordPolicy;
        }

        public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Name == null && request.Password == null)
                return Result<bool>.Failure("Aucune donnée à mettre à jour.");

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result<bool>.Failure("Utilisateur non trouvé.");

            if (string.IsNullOrEmpty(request.Name))
            {
                return Result<bool>.Failure("Le nom ne peut pas être vide.");
            }
            
            user.SetName(request.Name);
            
                
            if (request.Password != null)
            {
                //Vérifier mot de passe respecte la politique
                if (!await _passwordPolicy.ValidateAsync(request.Password))
                    return Result<bool>.Failure("Le mot de passe ne respecte pas la politique.");

                user.SetPasswordHash(_passwordHasher.HashPassword(user, request.Password));
            }

            _userRepository.Update(user);

            return Result<bool>.Success(true);
        }
    }
}