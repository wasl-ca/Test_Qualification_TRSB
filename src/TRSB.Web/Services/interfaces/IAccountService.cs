using System.Threading.Tasks;
using TRSB.Application.Common;
using TRSB.Web.Models;

namespace TRSB.Web.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Result<LoginResponse?>> Login(LoginViewModel model);
        Task<Result<bool>> Logout();
        Task<Result<bool>> Register(RegisterViewModel model);
        Task<Result<ProfileViewModel?>> GetProfileAsync();
        Task<Result<bool>> UpdateProfileAsync(UpdateProfileViewModel model);
    }
}
