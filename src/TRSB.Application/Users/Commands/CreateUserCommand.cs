using MediatR;
using TRSB.Application.Common;

namespace TRSB.Application.Users.Commands
{
    public record CreateUserCommand(
        string Username,
        string Name,
        string Email,
        string Password) : IRequest<Result<Guid>>;
}
