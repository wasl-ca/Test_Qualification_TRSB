// src/TRSB.Domain/Interfaces/IPasswordPolicy.cs
namespace TRSB.Domain.Interfaces;

public interface IPasswordPolicy
{
    Task<bool> ValidateAsync(string password);
}
