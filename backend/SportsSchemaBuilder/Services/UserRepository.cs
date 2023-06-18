using Microsoft.EntityFrameworkCore;
using SportsSchemaBuilder.Data;
using SportsSchemaBuilder.Models;
using SportsSchemaBuilder.Dto;
using System;

namespace SportsSchemaBuilder.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context)
        {
            _context = context;
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
