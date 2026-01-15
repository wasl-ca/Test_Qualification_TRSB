// src/TRSB.Infrastructure/Security/ConfigurablePasswordPolicy.cs
using TRSB.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using TRSB.Domain.Entities;

namespace TRSB.Infrastructure.Security;

public class ConfigurablePasswordPolicy : IPasswordPolicy
{
    private readonly IConfiguration _configuration;

    public ConfigurablePasswordPolicy(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task<bool> ValidateAsync(string password)
    {
        int minLength =  _configuration.GetValue<int?>("PasswordPolicy:MinLength") ?? 8;

        int minSpecial = _configuration.GetValue<int?>("PasswordPolicy:MinSpecialChars") ?? 2;

        var specialPattern = @"[@#!%&]";

        if (string.IsNullOrEmpty(password)) return false;
        if (password.Length < minLength) return false;

        var specialCount = Regex.Matches(password, specialPattern).Count;
        if (specialCount < minSpecial) return false;

        return true;
    }
}
