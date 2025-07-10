using Fin.Domain.Entities;

namespace Fin.Domain.Interfaces;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    User? GetUserById(int id);
    void Upsert(User user);
}