using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Repository;

public class UserRepository : IUserRepository
{
    private readonly FinDbContext _context;
    public UserRepository(FinDbContext context)
    {
        _context = context;
    }
    public User? GetUserByEmail(string email)
    {
        var users = _context.Users
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
        var users = _context.Users
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
            _context.Users.Update(existingUser);
        }
        else
        {
            _context.Users.Add(user);
        }

        _context.SaveChanges();
    }
}

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    User? GetUserById(int id);
    void Upsert(User user);
}
