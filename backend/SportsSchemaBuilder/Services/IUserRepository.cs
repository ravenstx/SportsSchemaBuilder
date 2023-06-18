using SportsSchemaBuilder.Models;
using SportsSchemaBuilder.Dto;

namespace SportsSchemaBuilder.Services
{
    public interface IUserRepository
    {
        //void GetUser(string name);
        Task<User> GetUser(string name);

        UserRefreshToken GetRefreshTokens(int id);

        void UpdateRefreshToken(UserRefreshToken token);
        void AddRefreshToken(UserRefreshToken token);

        void Add(User user);

        void Remove(User DbUser);

        Task SaveChangesAsync();
    }
}
