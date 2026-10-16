using Memo.Domain.Common;
using Memo.Domain.Exceptions;

namespace Memo.Domain.Entities;

public class Seller : AuditableEntity
{
    public string Name { get; private set; }
    public string Login { get; private set; }
    public string Password { get; private set; } 
    
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
            throw new DomainException("Name cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(login))
            throw new DomainException("Login cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Password cannot be null or empty.");
    }
}
