using TRSB.Domain.ValueObjects;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Username { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    // Value Object Email
    public string EmailValue { get; private set; } = null!;
    public Email Email => new Email(EmailValue);

    // Constructeur pour EF
    protected User() { }

    // Constructeur métier
    public User(string username, string name, Email email)
    {
        Username = username;
        Name = name;
        EmailValue = email.Address;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void SetName(string name)
    {
        Name = name;
    }
}
