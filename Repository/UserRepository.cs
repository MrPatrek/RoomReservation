using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await FindByCondition(user => user.Username.Equals(username))
                .FirstOrDefaultAsync();
        }

    }
}
