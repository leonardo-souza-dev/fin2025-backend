using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    User? Get(int id);
    User Create(User user);
}

public class UserRepository(FinDbContext context) : IUserRepository
{
    public User? GetUserByEmail(string email)
    {
        var user = context.Users.FirstOrDefault(u => u.Email == email);
        return user;
    }

    public User? Get(int id)
    {
        var user = context.Users.Find(id);
        return user != null && user.IsActive ? user : null;;
    }

    public User Create(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        user.IsActive = true;
        
        context.Users.Add(user);
        
        return user;
    }
}
