using Fin.Domain.Entities;
using Fin.Infrastructure.Data;

namespace Fin.Infrastructure.Repositories;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    User? GetUserById(int id);
    void Upsert(User user);
}

public class UserRepository(FinDbContext context) : IUserRepository
{
    public User? GetUserByEmail(string email)
    {
        var users = context.Users
            .Where(u => u.Email == email)
            .ToList();

        if (users.Count == 0)
        {
            return null;
        }

        if (users.Count > 1)
        {
            throw new InvalidOperationException($"Multiple users found with email: {email}");
        }

        return users.First();
    }

    public User? GetUserById(int id)
    {
        var users = context.Users
            .Where(u => u.Id == id)
            .ToList();

        if (users.Count == 0)
        {
            return null;
        }

        if (users.Count > 1)
        {
            throw new InvalidOperationException($"Multiple users found with Id: {id}");
        }

        return users.First();
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
