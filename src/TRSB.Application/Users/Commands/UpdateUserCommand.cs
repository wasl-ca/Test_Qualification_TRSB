using MediatR;
using TRSB.Application.Common;

namespace TRSB.Application.Users.Commands
{
    public record UpdateUserCommand(
        Guid UserId,
        string? Name,
        string? Password) : IRequest<Result<bool>>;

}