using MediatR;
using TRSB.Application.Common;

namespace TRSB.Application.Users.Commands
{
    public record LoginUserCommand(
        string UsernameOrEmail,
        string Password) : IRequest<Result<string>>; // retourne un token JWT
}
