using Fin.Api.Models;
using Fin.Api.Repository;

namespace Fin.Api.Services;

public class UserService(IUserRepository repository)
{
    private readonly IUserRepository _repository = repository;

    public User? GetUserByEmail(string email)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email), "Email cannot be null");
        }

        return _repository.GetUserByEmail(email);
    }

    public User? GetUserById(int id)
    {
        return _repository.GetUserById(id);
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

        _repository.Upsert(user);
    }
}
