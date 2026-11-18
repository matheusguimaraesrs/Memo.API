using Memo.Domain.Common;
using Memo.Domain.Exceptions;

namespace Memo.Domain.Entities;

public sealed class Seller : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Login { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    
    private Seller() { }

    public Seller(string name, string login, string password)
    {
        Validations(name, login, password);
        
        Name = name;
        Login = login;
        Password = password;
    }

    public void Update(string name, string login, string password)
    {
        Validations(name, login, password);
        
        Name = name;
        Login = login;
        Password = password;
    }

    private static void Validations(string name, string login, string password)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Seller name cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(login))
            throw new DomainException("Login cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Password cannot be null or empty.");
    }
}
