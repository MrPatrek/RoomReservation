using Entities.Models;

namespace Contracts
{
    public interface IUserRepository
    {
        User GetUserByUsername(string username);
    }
}
