using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using SportsSchemaBuilder.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsSchemaBuilder.test
{
    public class UserRepositoryFake: IUserRepository
    {
        private readonly UserContext _context;
        private readonly IAuthService _authService;

        public UserRepositoryFake(IAuthService _authService)
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new UserContext(options);
            databaseContext.Database.EnsureCreated();

            for (int i = 1; i <= 1; i++)
            {
                databaseContext.Users.Add(new User()
                {
                    Id = i,
                    Name = "test",
                    HashedPassword = _authService.HashPassword("pass")

                });
                databaseContext.SaveChangesAsync();
            }


            _context = databaseContext;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public void Remove(User DbUser)
        {
            _context.Users.Remove(DbUser);
        }

        public async Task<User> GetUser(string name)
        {
            return _context.Users.SingleOrDefault(e => e.Name == name);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public UserRefreshToken GetRefreshTokens(int id)
        {
            return _context.RefreshTokens.SingleOrDefault(e => e.UserId == id);

        }

        public void AddRefreshToken(UserRefreshToken token)
        {
            _context.RefreshTokens.Add(token);
        }
        public void UpdateRefreshToken(UserRefreshToken token)
        {
            _context.RefreshTokens.Update(token);
        }
    

}
}
