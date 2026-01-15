using System.Text.RegularExpressions;

namespace TRSB.Domain.ValueObjects;

public class Email
{
    public string Address { get; private set; }
    

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Le courriel est requis");

        if (!IsValidEmail(address))
            throw new ArgumentException("Format de courriel est invalide");

        Address = address;
    }

    private bool IsValidEmail(string email)
    {
        // Validation simple mais correcte
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public override string ToString() => Address;
}
