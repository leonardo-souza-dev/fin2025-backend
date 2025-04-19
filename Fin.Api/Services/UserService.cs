using Fin.Api.Data;
using Fin.Api.Models;

namespace Fin.Api.Services;

public class UserService(FinDbContext context)
{
    private readonly FinDbContext _context = context;

    public User? GetUserByEmail(string email)
    {
        if (email == null) 
            throw new ArgumentNullException(nameof(email), "Email cannot be null");

        var users = _context.Users
            .Where(u => u.Email == email)
            .ToList();

        if (users.Count == 0)
            return null;

        if (users.Count > 1)
            throw new InvalidOperationException($"Multiple users found with email: {email}");

        return users.First();
    }

    public User? GetUserById(int id)
    {
        var users = _context.Users
            .Where(u => u.Id == id)
            .ToList();

        if (users.Count == 0)
            return null;

        if (users.Count > 1)
            throw new InvalidOperationException($"Multiple users found with Id: {id}");

        return users.First();
    }

    public void Upsert(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }
        if (string.IsNullOrEmpty(user.Email))
        {
            throw new ArgumentException("Email cannot be null or empty", nameof(user.Email));
        }
        if (string.IsNullOrEmpty(user.Password))
        {
            throw new ArgumentException("PasswordHash cannot be null or empty", nameof(user.Password));
        }
        //TODO: Fazer validacoes no model

        var existingUser = GetUserByEmail(user.Email);
        if (existingUser != null)
        {
            existingUser.Password = user.Password;
            existingUser.Role = user.Role;
            existingUser.IsActive = user.IsActive;
            _context.Users.Update(existingUser);
        }
        else
        {
            _context.Users.Add(user);
        }
        _context.SaveChanges();
    }
}
