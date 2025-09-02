using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases;

public class UserService(IUserRepository repository, IUnitOfWork unitOfWork)
{
    public User? GetUserByEmail(string email)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email), "Email cannot be null");
        }

        var user = repository.GetUserByEmail(email);
        
        return user;
    }

    public User Create(User user)
    {
        _ = repository.Create(user);
        
        unitOfWork.SaveChanges();
        return user;
    }
}
