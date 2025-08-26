using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    User? Get(int id);
    void Upsert(User user);
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

    public void Upsert(User user)
    {
        var existingUser = GetUserByEmail(user.Email);

        if (existingUser != null)
        {
            existingUser.Email = user.Email;
            existingUser.IsActive = user.IsActive;
            existingUser.Role = user.Role;
            context.Users.Update(existingUser);
        }
        else
        {
            context.Users.Add(user);
        }
    }
}
