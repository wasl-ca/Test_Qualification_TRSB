using MediatR;
using TRSB.Application.Common;

namespace TRSB.Application.Users.Queries
{
    public record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileDto>>;
}
