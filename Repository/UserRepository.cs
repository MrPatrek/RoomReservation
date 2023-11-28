using Contracts;
using Entities;
using Entities.Models;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public User GetUserByUsername(string username)
        {
            return FindByCondition(user => user.Username.Equals(username))
                .FirstOrDefault();
        }

    }
}
