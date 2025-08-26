using Fin.Domain.Entities;
using Fin.Domain.Exceptions;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases;

public class UserService(IUserRepository repository)
{
    public User? GetUserByEmail(string email)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email), "Email cannot be null");
        }

        var user = repository.GetUserByEmail(email);

        if (user == null)
        {
            throw new UserNotFoundException(nameof(email), email);
        }
        
        return user;
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

        repository.Upsert(user);
    }
}
