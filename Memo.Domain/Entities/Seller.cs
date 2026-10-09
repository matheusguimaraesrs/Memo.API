using Memo.Domain.Common;

namespace Memo.Domain.Entities;

public class Seller : AuditableEntity
{
    public string Name { get; private set; }
    public string Login { get; private set; }
    public byte[] PasswordHash { get; private set; } 
    public byte[] PasswordSalt { get; private set; }

    public Seller(string name, string login, byte[] passwordHash, byte[] passwordSalt)
    {
        Name = name;
        Login = login;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public void UpdateSeller(string name, string login)
    {
        Name = name;
        Login = login;
    }
}
