using MediatR;
using TRSB.Application.Common;
using TRSB.Domain.Interfaces;

namespace TRSB.Application.Users.Queries
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        private readonly IRepository<User> _userRepository;

        public GetUserProfileQueryHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = (await _userRepository.FindAsync(u => u.Id == request.UserId))
                .FirstOrDefault();

            if (user is null)
                return Result<UserProfileDto>.Failure("Utilisateur non trouvé");

            return Result<UserProfileDto>.Success(new UserProfileDto
            {
                Username = user.Username,
                Name = user.Name,
                Email = user.EmailValue
            });
        }
    }

    public class UserProfileDto
    {
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
