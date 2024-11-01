using Microsoft.EntityFrameworkCore;
using ZSports.Core.Interfaces.Repositories;
using ZSports.Domain.User;
using ZSports.Repository.Data;

namespace ZSports.Repository.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User> GetUserByEmailAsync(string email) 
            => await _context.Users.FirstOrDefaultAsync(x => x.Email == email)!;
    }
}
